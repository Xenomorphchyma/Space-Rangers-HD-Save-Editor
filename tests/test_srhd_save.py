from __future__ import annotations

import struct
import sys
import unittest
import zlib
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from srhd_save import (  # noqa: E402
    INNER_METADATA_SIZE,
    GalaxyDirectory,
    InnerMetadata,
    SavContainer,
    SavFormatError,
    crypt_bytes,
    patch_inner_metadata,
    write_metadata_patch,
    _scan_star_headers,
)


def zl01(payload: bytes) -> bytes:
    return b"ZL01" + struct.pack("<I", len(payload)) + zlib.compress(payload, 9)


class SrhdSaveTests(unittest.TestCase):
    def build_container(self) -> bytes:
        galaxy_prefix = (
            b"\x00\x00"
            + struct.pack(
                "<iIiiffBBiiiiH",
                1,
                2,
                3,
                4,
                5.0,
                6.0,
                0,
                0,
                0,
                7,
                8,
                9,
                0,
            )
        )
        main_payload = (
            struct.pack("<III", 7, 100, 200)
            + bytes((1, 0, 1, 0, 0, 0, 0, 1))
            + struct.pack("<III", 3, 0, 0)
            + struct.pack("<H", 0)
            + galaxy_prefix
            + struct.pack("<HH", 0, 0)
            + b"synthetic-main-data"
        )
        main_block = zl01(main_payload)
        key = 1_234_567
        preview = zl01(struct.pack("<III", 2, 6, 2) + bytes(range(12)))
        map_block = zl01(struct.pack("<III", 1, 3, 1) + b"\x01\x02\x03")
        container = SavContainer(
            source_size=0,
            header=("RSG", "v167", "Synthetic", "1", "2", "Ranger", "People", "EZ"),
            preview_block=preview,
            map_block=map_block,
            stored_crc32=zlib.crc32(main_block) & 0xFFFFFFFF,
            encryption_key=key,
            encrypted_main_block=crypt_bytes(main_block, key),
            film_block=b"film-data",
            offsets={},
        )
        return container.serialize()

    def test_parse_validate_and_serialize_is_lossless(self) -> None:
        data = self.build_container()
        container = SavContainer.parse(data)
        report = container.inspect()
        self.assertEqual(data, container.serialize())
        self.assertTrue(report["main_block"]["crc_valid"])
        self.assertTrue(report["main_block"]["valid"])
        self.assertEqual(7, report["main_block"]["metadata_prefix"]["current_form"])
        self.assertEqual(1, report["main_block"]["galaxy_prefix"]["random_seed"])
        self.assertEqual(0, report["main_block"]["galaxy_directory"]["star_count"])

    def test_crypt_is_symmetric(self) -> None:
        payload = bytes(range(256))
        encrypted = crypt_bytes(payload, 987_654_321)
        self.assertNotEqual(payload, encrypted)
        self.assertEqual(payload, crypt_bytes(encrypted, 987_654_321))

    def test_crc_failure_is_reported(self) -> None:
        data = bytearray(self.build_container())
        container = SavContainer.parse(bytes(data))
        bad = SavContainer(
            source_size=container.source_size,
            header=container.header,
            preview_block=container.preview_block,
            map_block=container.map_block,
            stored_crc32=container.stored_crc32 ^ 1,
            encryption_key=container.encryption_key,
            encrypted_main_block=container.encrypted_main_block,
            film_block=container.film_block,
            offsets=container.offsets,
        )
        report = bad.inspect()
        self.assertFalse(report["valid"])
        self.assertFalse(report["main_block"]["crc_valid"])

    def test_ordered_star_headers_are_found_without_parsing_nested_objects(self) -> None:
        def star(object_id: int, name: str, x: float, y: float, planets: int) -> bytes:
            return (
                struct.pack("<IiI", object_id, -object_id, object_id * 100)
                + name.encode("utf-16le")
                + b"\x00\x00"
                + struct.pack("<ffHBH", x, y, 200, object_id + 10, planets)
            )

        payload = b"prefix" + star(1, "Тарон", 12.0, 34.0, 7) + b"nested-object-data" + star(2, "Пхедок", 56.0, 78.0, 5) + b"tail"
        headers = _scan_star_headers(payload, len(b"prefix"), 2)
        self.assertEqual(["Тарон", "Пхедок"], [item.name for item in headers])
        self.assertEqual([7, 5], [item.planet_count for item in headers])

    def test_constellation_references_and_map_lines_are_decoded(self) -> None:
        prefix = b"\x00\x00" + struct.pack(
            "<iIiiffBBiiiiH", 1, 2, 3, 4, 5.0, 6.0, 0, 0, 0, 0, 0, 0, 0
        )
        constellation = (
            struct.pack("<IBHff", 7, 1, 0x1234, 42.5, -9.25)
            + struct.pack("<HII", 2, 11, 12)
            + struct.pack("<HI", 1, 8)
            + struct.pack("<HH", 0, 0)
            + struct.pack("<iiiiii", 0, 0, 0, 0, 0, 0)
            + struct.pack("<Hffff", 1, 1.0, 2.0, 3.0, 4.0)
            + struct.pack("<HH", 0, 0)
        )
        directory = GalaxyDirectory.parse(prefix + struct.pack("<H", 1) + constellation + struct.pack("<H", 0), 0, 167)
        item = directory.constellations[0]
        self.assertEqual((11, 12), item.star_object_ids)
        self.assertEqual((8,), item.connection_object_ids)
        self.assertEqual(((1.0, 2.0, 3.0, 4.0),), item.map_lines)

    def test_metadata_patch_preserves_opaque_model_bytes(self) -> None:
        container = SavContainer.parse(self.build_container())
        original = container.main_payload()
        patched, report = patch_inner_metadata(
            original,
            {
                "camera_x": -321,
                "camera_y": 654,
                "show_panel": False,
                "view_follow": True,
                "calc_header": True,
                "tips": 0x12345678,
            },
        )
        metadata = InnerMetadata.parse(patched)
        self.assertEqual(-321, metadata.camera_x)
        self.assertEqual(654, metadata.camera_y)
        self.assertFalse(metadata.show_panel)
        self.assertTrue(metadata.view_follow)
        self.assertTrue(metadata.calc_header)
        self.assertEqual(0x12345678, metadata.tips)
        self.assertEqual(original[INNER_METADATA_SIZE:], patched[INNER_METADATA_SIZE:])
        self.assertTrue(report["opaque_bytes_preserved"])

    def test_metadata_patch_rejects_unknown_reserved_layout(self) -> None:
        container = SavContainer.parse(self.build_container())
        payload = bytearray(container.main_payload())
        payload[13] = 1
        with self.assertRaises(SavFormatError):
            patch_inner_metadata(bytes(payload), {"camera_x": 1})

    def test_metadata_patch_writes_new_verified_container(self) -> None:
        from tempfile import TemporaryDirectory

        with TemporaryDirectory() as directory:
            input_path = Path(directory) / "input.sav"
            output_path = Path(directory) / "output.sav"
            input_path.write_bytes(self.build_container())
            result = write_metadata_patch(
                input_path,
                output_path,
                {"camera_x": -100, "tips": 0xFF},
            )
            self.assertTrue(result["verified"])
            self.assertTrue(result["opaque_model_bytes_preserved"])
            self.assertTrue(output_path.exists())
            with self.assertRaises(FileExistsError):
                write_metadata_patch(input_path, output_path, {"camera_x": 2})


if __name__ == "__main__":
    unittest.main()

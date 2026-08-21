#!/usr/bin/env python3
"""SRHD SAV container inspector and safe metadata editor.

The default commands never modify an input file.  ``roundtrip`` writes a new
file and deliberately refuses to replace an existing path.  The parser proves
the inner stream through player messages, player hold, the current TGalaxy
prefix, custom weapons, constellations and the start of the TStar list.  Writer
operations preserve every unmodified byte, including unknown modded tails.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import math
import struct
import sys
import zlib
from dataclasses import dataclass, replace
from pathlib import Path
from typing import Any, Sequence


HEADER_FIELDS = (
    "signature",
    "version_text",
    "save_name",
    "current_turn_text",
    "player_money_text",
    "player_name",
    "player_race",
    "post_signature",
)
ZL01_MAGIC = b"ZL01"
MAX_HEADER_STRING_CODE_UNITS = 1 << 16
SUPPORTED_PATCH_VERSIONS = frozenset((166, 167))
INNER_METADATA_SIZE = 32
INNER_METADATA_RESERVED = {
    13: 0,
    15: 0,
    16: 0,
    17: 0,
    18: 0,
}
INNER_METADATA_FIELD_RANGES = {
    "camera_x": range(4, 8),
    "camera_y": range(8, 12),
    "show_panel": range(12, 13),
    "view_follow": range(14, 15),
    "calc_header": range(19, 20),
    "tips": range(20, 24),
}


class SavFormatError(ValueError):
    """Raised when a SAV container cannot be parsed without guessing."""


class _Reader:
    def __init__(self, data: bytes) -> None:
        self.data = data
        self.offset = 0

    @property
    def remaining(self) -> int:
        return len(self.data) - self.offset

    def take(self, size: int, label: str) -> bytes:
        if size < 0 or size > self.remaining:
            raise SavFormatError(
                f"{label}: requested {size} bytes at 0x{self.offset:X}, "
                f"only {self.remaining} remain"
            )
        start = self.offset
        self.offset += size
        return self.data[start : start + size]

    def u32(self, label: str) -> int:
        return struct.unpack("<I", self.take(4, label))[0]

    def i32(self, label: str) -> int:
        return struct.unpack("<i", self.take(4, label))[0]

    def f32(self, label: str) -> float:
        return struct.unpack("<f", self.take(4, label))[0]

    def u16(self, label: str) -> int:
        return struct.unpack("<H", self.take(2, label))[0]

    def u8(self, label: str) -> int:
        return self.take(1, label)[0]

    def boolean(self, label: str) -> bool:
        value = self.u8(label)
        if value not in (0, 1):
            raise SavFormatError(
                f"{label}: expected Boolean byte 0 or 1 at 0x{self.offset - 1:X}, "
                f"got {value}"
            )
        return bool(value)

    def utf16z(self, label: str) -> str:
        start = self.offset
        for _ in range(MAX_HEADER_STRING_CODE_UNITS):
            unit = self.take(2, label)
            if unit == b"\x00\x00":
                raw = self.data[start : self.offset - 2]
                try:
                    return raw.decode("utf-16le")
                except UnicodeDecodeError as error:
                    raise SavFormatError(
                        f"{label}: invalid UTF-16LE at 0x{start:X}: {error}"
                    ) from error
        raise SavFormatError(f"{label}: missing UTF-16 terminator at 0x{start:X}")


def _utf16z(value: str) -> bytes:
    return value.encode("utf-16le") + b"\x00\x00"


def _sha256(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest().upper()


def crypt_bytes(data: bytes, key: int) -> bytes:
    """Apply the save format's symmetric Park-Miller XOR stream."""

    seed = key & 0xFFFFFFFF
    output = bytearray(data)
    for index, value in enumerate(output):
        seed = (seed % 127_773) * 16_807 - (seed // 127_773) * 2_836
        if seed < 1:
            seed += 2_147_483_647
        output[index] = value ^ ((seed - 1) & 0xFF)
    return bytes(output)


@dataclass(frozen=True)
class Zl01Result:
    expected_size: int
    actual_size: int
    payload: bytes
    trailing_bytes: int


def decompress_zl01(block: bytes, label: str) -> Zl01Result:
    if len(block) < 8:
        raise SavFormatError(f"{label}: ZL01 block is only {len(block)} bytes")
    if block[:4] != ZL01_MAGIC:
        raise SavFormatError(
            f"{label}: expected ZL01 magic, got {block[:4].hex(' ').upper()}"
        )
    expected_size = struct.unpack_from("<I", block, 4)[0]
    decoder = zlib.decompressobj()
    try:
        payload = decoder.decompress(block[8:]) + decoder.flush()
    except zlib.error as error:
        raise SavFormatError(f"{label}: zlib decompression failed: {error}") from error
    if not decoder.eof:
        raise SavFormatError(f"{label}: truncated zlib stream")
    trailing = len(decoder.unused_data)
    if len(payload) != expected_size:
        raise SavFormatError(
            f"{label}: expected {expected_size} unpacked bytes, got {len(payload)}"
        )
    return Zl01Result(expected_size, len(payload), payload, trailing)


def compress_zl01(payload: bytes) -> bytes:
    """Build the ZL01 form used by Space Rangers HD (zlib level 9)."""

    if len(payload) > 0xFFFFFFFF:
        raise SavFormatError("main_block: unpacked payload is too large for ZL01")
    return ZL01_MAGIC + struct.pack("<I", len(payload)) + zlib.compress(payload, 9)


@dataclass(frozen=True)
class InnerMetadata:
    """Proven fixed prefix written before player data and ``TGalaxy``."""

    current_form: int
    camera_x: int
    camera_y: int
    show_panel: bool
    view_follow: bool
    calc_header: bool
    tips: int
    player_message_count: int
    raw_flags: tuple[int, ...]
    reserved_u32: int

    @classmethod
    def parse(cls, payload: bytes) -> "InnerMetadata":
        if len(payload) < INNER_METADATA_SIZE:
            raise SavFormatError(
                f"main_block: need at least {INNER_METADATA_SIZE} metadata bytes, "
                f"got {len(payload)}"
            )
        for offset, expected in INNER_METADATA_RESERVED.items():
            actual = payload[offset]
            if actual != expected:
                raise SavFormatError(
                    f"main_block: reserved metadata byte at 0x{offset:X} is "
                    f"0x{actual:02X}, expected 0x{expected:02X}; refusing to guess"
                )
        for offset, name in ((12, "show_panel"), (14, "view_follow"), (19, "calc_header")):
            if payload[offset] not in (0, 1):
                raise SavFormatError(
                    f"main_block: {name} at 0x{offset:X} is {payload[offset]}, expected 0 or 1"
                )
        return cls(
            current_form=struct.unpack_from("<i", payload, 0)[0],
            camera_x=struct.unpack_from("<i", payload, 4)[0],
            camera_y=struct.unpack_from("<i", payload, 8)[0],
            show_panel=bool(payload[12]),
            view_follow=bool(payload[14]),
            calc_header=bool(payload[19]),
            tips=struct.unpack_from("<I", payload, 20)[0],
            player_message_count=struct.unpack_from("<I", payload, 28)[0],
            raw_flags=tuple(payload[12:20]),
            reserved_u32=struct.unpack_from("<I", payload, 24)[0],
        )

    def as_dict(self) -> dict[str, Any]:
        return {
            "current_form": self.current_form,
            "camera_x": self.camera_x,
            "camera_y": self.camera_y,
            "show_panel": self.show_panel,
            "view_follow": self.view_follow,
            "calc_header": self.calc_header,
            "tips": self.tips,
            "tips_hex": f"0x{self.tips:08X}",
            "player_message_count": self.player_message_count,
            "raw_flags": list(self.raw_flags),
            "reserved_u32": self.reserved_u32,
        }


@dataclass(frozen=True)
class PlayerMessageEnvelope:
    """Sequential fields consumed by ``TMessagePlayer.Read``.

    Field names that are not yet proven from UI bindings intentionally keep
    their object-offset names.  The span is exact even when the semantic name
    is pending.
    """

    start: int
    end: int
    text: str
    message_type: int
    raw_18: int
    raw_1c: int
    formatted_text: str
    raw_bool: bool
    raw_u32: tuple[int, int, int, int, int, int]
    flags: tuple[bool, bool]
    late_text: str | None


@dataclass(frozen=True)
class PlayerHoldUnit:
    start: int
    end: int
    unit_type: int
    goods: int
    object_id: int


@dataclass(frozen=True)
class InnerEnvelope:
    """The proven stream between metadata offset 0 and ``TGalaxy.Read``."""

    metadata: InnerMetadata
    messages: tuple[PlayerMessageEnvelope, ...]
    hold_units: tuple[PlayerHoldUnit, ...]
    galaxy_offset: int
    payload_size: int

    @classmethod
    def parse(cls, payload: bytes, version: int) -> "InnerEnvelope":
        metadata = InnerMetadata.parse(payload)
        reader = _Reader(payload)
        reader.offset = INNER_METADATA_SIZE
        messages: list[PlayerMessageEnvelope] = []
        for index in range(metadata.player_message_count):
            start = reader.offset
            text = reader.utf16z(f"player_message[{index}].text")
            message_type = reader.u8(f"player_message[{index}].type")
            raw_18 = reader.i32(f"player_message[{index}].raw_18")
            raw_1c = reader.i32(f"player_message[{index}].raw_1c")
            formatted_text = reader.utf16z(f"player_message[{index}].formatted_text")
            raw_bool = reader.boolean(f"player_message[{index}].raw_bool")
            raw_u32 = tuple(
                reader.u32(f"player_message[{index}].u32_{offset:02X}")
                for offset in (0x24, 0x2C, 0x34, 0x28, 0x30, 0x38)
            )
            flags = (
                reader.boolean(f"player_message[{index}].flag_40"),
                reader.boolean(f"player_message[{index}].flag_41"),
            )
            late_text = (
                reader.utf16z(f"player_message[{index}].late_text")
                if version > 108
                else None
            )
            messages.append(
                PlayerMessageEnvelope(
                    start=start,
                    end=reader.offset,
                    text=text,
                    message_type=message_type,
                    raw_18=raw_18,
                    raw_1c=raw_1c,
                    formatted_text=formatted_text,
                    raw_bool=raw_bool,
                    raw_u32=raw_u32,  # type: ignore[arg-type]
                    flags=flags,
                    late_text=late_text,
                )
            )

        hold_count = reader.u16("player_hold_count")
        hold_units: list[PlayerHoldUnit] = []
        for index in range(hold_count):
            start = reader.offset
            hold_units.append(
                PlayerHoldUnit(
                    start=start,
                    end=start + 6,
                    unit_type=reader.u8(f"player_hold[{index}].type"),
                    goods=reader.u8(f"player_hold[{index}].goods"),
                    object_id=reader.u32(f"player_hold[{index}].object_id"),
                )
            )
        return cls(
            metadata=metadata,
            messages=tuple(messages),
            hold_units=tuple(hold_units),
            galaxy_offset=reader.offset,
            payload_size=len(payload),
        )

    def as_dict(self) -> dict[str, Any]:
        return {
            "metadata_size": INNER_METADATA_SIZE,
            "player_message_count": len(self.messages),
            "player_messages_span": [
                f"0x{INNER_METADATA_SIZE:X}",
                f"0x{(self.messages[-1].end if self.messages else INNER_METADATA_SIZE):X}",
            ],
            "player_hold_count": len(self.hold_units),
            "galaxy_offset": self.galaxy_offset,
            "galaxy_offset_hex": f"0x{self.galaxy_offset:X}",
            "galaxy_bytes": self.payload_size - self.galaxy_offset,
        }


@dataclass(frozen=True)
class GalaxyPrefix:
    """Current-schema scalar prefix emitted first by ``TGalaxy.Write``.

    The boundary ends immediately after the custom-weapon count.  Weapon
    records and every following object are parsed separately or byte-preserved.
    """

    start: int
    end: int
    used_mods: str
    random_seed: int
    random_out: int
    rangers_average_capital: int
    rangers_max_capital: int
    rangers_average_strength: float
    rangers_max_strength: float
    crack: bool
    cheat: bool
    reserved_zero: int
    cheat_points: int
    save_count: int
    load_count: int
    custom_mod_weapon_count: int

    @classmethod
    def parse(cls, payload: bytes, start: int, version: int) -> "GalaxyPrefix":
        if version < 127:
            raise SavFormatError(
                f"TGalaxy scalar prefix is only proven for current SAV schema, got v{version}"
            )
        reader = _Reader(payload)
        reader.offset = start
        value = cls(
            start=start,
            end=0,
            used_mods=reader.utf16z("galaxy.used_mods") if version > 100 else "",
            random_seed=reader.i32("galaxy.random_seed"),
            random_out=reader.u32("galaxy.random_out"),
            rangers_average_capital=reader.i32("galaxy.rangers_average_capital"),
            rangers_max_capital=reader.i32("galaxy.rangers_max_capital"),
            rangers_average_strength=reader.f32("galaxy.rangers_average_strength"),
            rangers_max_strength=reader.f32("galaxy.rangers_max_strength"),
            crack=reader.boolean("galaxy.crack"),
            cheat=reader.boolean("galaxy.cheat"),
            reserved_zero=reader.i32("galaxy.reserved_zero"),
            cheat_points=reader.i32("galaxy.cheat_points"),
            save_count=reader.i32("galaxy.save_count"),
            load_count=reader.i32("galaxy.load_count"),
            custom_mod_weapon_count=reader.u16("galaxy.custom_mod_weapon_count"),
        )
        if value.reserved_zero != 0:
            raise SavFormatError(
                f"galaxy.reserved_zero at 0x{start:X} is {value.reserved_zero}, expected 0"
            )
        return replace(value, end=reader.offset)

    @property
    def used_mod_count(self) -> int:
        return len([item for item in self.used_mods.split(", ") if item])

    def as_dict(self) -> dict[str, Any]:
        return {
            "start": self.start,
            "start_hex": f"0x{self.start:X}",
            "end": self.end,
            "end_hex": f"0x{self.end:X}",
            "used_mods": self.used_mods,
            "used_mod_count": self.used_mod_count,
            "random_seed": self.random_seed,
            "random_out": self.random_out,
            "rangers_average_capital": self.rangers_average_capital,
            "rangers_max_capital": self.rangers_max_capital,
            "rangers_average_strength": self.rangers_average_strength,
            "rangers_max_strength": self.rangers_max_strength,
            "crack": self.crack,
            "cheat": self.cheat,
            "cheat_points": self.cheat_points,
            "save_count": self.save_count,
            "load_count": self.load_count,
            "custom_mod_weapon_count": self.custom_mod_weapon_count,
        }


@dataclass(frozen=True)
class CustomWeaponEnvelope:
    start: int
    end: int
    system_name: str
    weapon_type: int
    weapon_subtype: int


@dataclass(frozen=True)
class ConstellationEnvelope:
    start: int
    end: int
    object_id: int
    visible: bool
    color: int
    x: float
    y: float
    star_object_ids: tuple[int, ...]
    connection_object_ids: tuple[int, ...]
    map_lines: tuple[tuple[float, float, float, float], ...]

    def as_dict(self) -> dict[str, Any]:
        return {
            "start": self.start,
            "end": self.end,
            "object_id": self.object_id,
            "visible": self.visible,
            "color": self.color,
            "x": self.x,
            "y": self.y,
            "star_object_ids": list(self.star_object_ids),
            "connection_object_ids": list(self.connection_object_ids),
            "map_lines": [list(line) for line in self.map_lines],
        }


@dataclass(frozen=True)
class StarHeader:
    """Validated fixed header at the start of a current-schema ``TStar``."""

    start: int
    header_end: int
    object_id: int
    raw_08: int
    raw_0c: int
    name: str
    x: float
    y: float
    raw_1c: int
    raw_78: int
    planet_count: int

    def as_dict(self) -> dict[str, Any]:
        return {
            "start": self.start,
            "start_hex": f"0x{self.start:X}",
            "header_end": self.header_end,
            "object_id": self.object_id,
            "name": self.name,
            "x": self.x,
            "y": self.y,
            "planet_count": self.planet_count,
        }


@dataclass(frozen=True)
class GalaxyDirectory:
    """Proven TGalaxy sections through the start of the TStar object list."""

    prefix: GalaxyPrefix
    custom_weapons: tuple[CustomWeaponEnvelope, ...]
    constellations: tuple[ConstellationEnvelope, ...]
    star_count: int
    stars_offset: int
    star_headers: tuple[StarHeader, ...]

    @classmethod
    def parse(cls, payload: bytes, start: int, version: int) -> "GalaxyDirectory":
        prefix = GalaxyPrefix.parse(payload, start, version)
        reader = _Reader(payload)
        reader.offset = prefix.end
        weapons: list[CustomWeaponEnvelope] = []
        for index in range(prefix.custom_mod_weapon_count):
            record_start = reader.offset
            system_name = reader.utf16z(f"galaxy.custom_weapon[{index}].system_name")
            weapon_type = reader.u8(f"galaxy.custom_weapon[{index}].type")
            weapon_subtype = reader.u8(f"galaxy.custom_weapon[{index}].subtype")
            reader.f32(f"galaxy.custom_weapon[{index}].float_0C")
            for field in range(8):
                reader.i32(f"galaxy.custom_weapon[{index}].i32_{0x10 + field * 4:02X}")
            reader.u8(f"galaxy.custom_weapon[{index}].byte_30")
            reader.u32(f"galaxy.custom_weapon[{index}].u32_31")
            for field in range(3):
                reader.u8(f"galaxy.custom_weapon[{index}].byte_{0x35 + field:02X}")
            for field in range(10):
                reader.f32(f"galaxy.custom_weapon[{index}].float_{0x38 + field * 4:02X}")
            for field in range(3):
                present = reader.boolean(f"galaxy.custom_weapon[{index}].string_{field}.present")
                if present:
                    reader.utf16z(f"galaxy.custom_weapon[{index}].string_{field}")
            reader.i32(f"galaxy.custom_weapon[{index}].i32_6C")
            reader.u8(f"galaxy.custom_weapon[{index}].byte_70")
            reader.u8(f"galaxy.custom_weapon[{index}].byte_71")
            weapons.append(
                CustomWeaponEnvelope(
                    start=record_start,
                    end=reader.offset,
                    system_name=system_name,
                    weapon_type=weapon_type,
                    weapon_subtype=weapon_subtype,
                )
            )

        constellation_count = _bounded_stream_count(
            reader.u16("galaxy.constellation_count"), "galaxy.constellation_count"
        )
        constellations: list[ConstellationEnvelope] = []
        for index in range(constellation_count):
            record_start = reader.offset
            object_id = reader.u32(f"galaxy.constellation[{index}].id")
            visible = reader.boolean(f"galaxy.constellation[{index}].visible")
            color = reader.u16(f"galaxy.constellation[{index}].color")
            x = reader.f32(f"galaxy.constellation[{index}].x")
            y = reader.f32(f"galaxy.constellation[{index}].y")
            star_refs = _read_u32_list(reader, "star_refs", index)
            connection_refs = _read_u32_list(reader, "connection_refs", index)
            _read_map_line_list(reader, "line_group_1", index)
            _read_map_line_list(reader, "line_group_2", index)
            for field in range(6):
                reader.i32(f"galaxy.constellation[{index}].i32_{0x2C + field * 4:02X}")
            map_lines = _read_map_line_list(reader, "map_lines", index)
            _skip_polygon_list(reader, "polygon_group_1", index)
            _skip_polygon_list(reader, "polygon_group_2", index)
            constellations.append(
                ConstellationEnvelope(
                    start=record_start,
                    end=reader.offset,
                    object_id=object_id,
                    visible=visible,
                    color=color,
                    x=x,
                    y=y,
                    star_object_ids=star_refs,
                    connection_object_ids=connection_refs,
                    map_lines=map_lines,
                )
            )
        star_count = _bounded_stream_count(
            reader.u16("galaxy.star_count"), "galaxy.star_count"
        )
        stars_offset = reader.offset
        star_headers = _scan_star_headers(payload, stars_offset, star_count)
        return cls(
            prefix=prefix,
            custom_weapons=tuple(weapons),
            constellations=tuple(constellations),
            star_count=star_count,
            stars_offset=stars_offset,
            star_headers=star_headers,
        )

    def as_dict(self) -> dict[str, Any]:
        return {
            "custom_weapon_count": len(self.custom_weapons),
            "custom_weapon_span": [self.prefix.end, self.custom_weapons[-1].end if self.custom_weapons else self.prefix.end],
            "constellation_count": len(self.constellations),
            "constellations": [item.as_dict() for item in self.constellations],
            "star_count": self.star_count,
            "stars_offset": self.stars_offset,
            "stars_offset_hex": f"0x{self.stars_offset:X}",
            "star_headers_validated": len(self.star_headers),
            "stars": [item.as_dict() for item in self.star_headers],
            "custom_weapon_names": [item.system_name for item in self.custom_weapons],
        }


def _bounded_stream_count(value: int, label: str, maximum: int = 10_000) -> int:
    if value > maximum:
        raise SavFormatError(f"{label}: {value} exceeds proven limit {maximum}")
    return value


def _skip_fixed_list(reader: _Reader, section: str, index: int, item_size: int) -> int:
    count = _bounded_stream_count(
        reader.u16(f"galaxy.constellation[{index}].{section}.count"),
        f"galaxy.constellation[{index}].{section}.count",
    )
    reader.take(count * item_size, f"galaxy.constellation[{index}].{section}")
    return count


def _read_u32_list(reader: _Reader, section: str, index: int) -> tuple[int, ...]:
    count = _bounded_stream_count(
        reader.u16(f"galaxy.constellation[{index}].{section}.count"),
        f"galaxy.constellation[{index}].{section}.count",
    )
    return tuple(
        reader.u32(f"galaxy.constellation[{index}].{section}[{item}]")
        for item in range(count)
    )


def _read_map_line_list(
    reader: _Reader, section: str, index: int
) -> tuple[tuple[float, float, float, float], ...]:
    count = _bounded_stream_count(
        reader.u16(f"galaxy.constellation[{index}].{section}.count"),
        f"galaxy.constellation[{index}].{section}.count",
    )
    return tuple(
        (
            reader.f32(f"galaxy.constellation[{index}].{section}[{item}].x1"),
            reader.f32(f"galaxy.constellation[{index}].{section}[{item}].y1"),
            reader.f32(f"galaxy.constellation[{index}].{section}[{item}].x2"),
            reader.f32(f"galaxy.constellation[{index}].{section}[{item}].y2"),
        )
        for item in range(count)
    )


def _skip_polygon_list(reader: _Reader, section: str, index: int) -> None:
    polygon_count = _bounded_stream_count(
        reader.u16(f"galaxy.constellation[{index}].{section}.count"),
        f"galaxy.constellation[{index}].{section}.count",
    )
    for polygon in range(polygon_count):
        point_count = _bounded_stream_count(
            reader.u16(f"galaxy.constellation[{index}].{section}[{polygon}].point_count"),
            f"galaxy.constellation[{index}].{section}[{polygon}].point_count",
        )
        reader.take(
            point_count * 8 + 24,
            f"galaxy.constellation[{index}].{section}[{polygon}]",
        )


def _is_star_name_character(value: str) -> bool:
    return (
        value in " -'()."
        or "0" <= value <= "9"
        or "A" <= value <= "Z"
        or "a" <= value <= "z"
        or "\u0400" <= value <= "\u052f"
    )


def _star_header_candidate(payload: bytes, start: int, expected_id: int) -> StarHeader | None:
    if start + 32 > len(payload) or struct.unpack_from("<I", payload, start)[0] != expected_id:
        return None
    raw_08, raw_0c = struct.unpack_from("<iI", payload, start + 4)
    position = start + 12
    characters: list[str] = []
    for _ in range(64):
        if position + 2 > len(payload):
            return None
        code_unit = struct.unpack_from("<H", payload, position)[0]
        position += 2
        if code_unit == 0:
            break
        character = chr(code_unit)
        if not _is_star_name_character(character):
            return None
        characters.append(character)
    else:
        return None
    if len(characters) < 2 or position + 13 > len(payload):
        return None
    x, y, raw_1c, raw_78, planet_count = struct.unpack_from("<ffHBH", payload, position)
    if not (
        math.isfinite(x)
        and math.isfinite(y)
        and x == int(x)
        and y == int(y)
        and -4096 <= x <= 4096
        and -4096 <= y <= 4096
        and 1 <= raw_1c <= 4096
        and 1 <= planet_count <= 64
    ):
        return None
    return StarHeader(
        start=start,
        header_end=position + 13,
        object_id=expected_id,
        raw_08=raw_08,
        raw_0c=raw_0c,
        name="".join(characters),
        x=x,
        y=y,
        raw_1c=raw_1c,
        raw_78=raw_78,
        planet_count=planet_count,
    )


def _scan_star_headers(payload: bytes, start: int, count: int) -> tuple[StarHeader, ...]:
    """Locate every TStar header and reject ambiguous heuristic matches.

    ``TStar.Write`` proves the fixed header layout, while the ordered ids and
    exact count make the scan safe for read-only navigation before all nested
    polymorphic objects have been decoded.
    """

    matches: list[list[StarHeader]] = [[] for _ in range(count + 1)]
    for offset in range(start, len(payload) - 31):
        object_id = struct.unpack_from("<I", payload, offset)[0]
        if not 1 <= object_id <= count:
            continue
        candidate = _star_header_candidate(payload, offset, object_id)
        if candidate is not None:
            matches[object_id].append(candidate)

    result: list[StarHeader] = []
    previous = start - 1
    for object_id in range(1, count + 1):
        ordered = [item for item in matches[object_id] if item.start > previous]
        if len(ordered) != 1:
            raise SavFormatError(
                f"galaxy.star[{object_id}]: expected one ordered header, found {len(ordered)}"
            )
        result.append(ordered[0])
        previous = ordered[0].start
    return tuple(result)


@dataclass(frozen=True)
class SavContainer:
    source_size: int
    header: tuple[str, ...]
    preview_block: bytes
    map_block: bytes
    stored_crc32: int
    encryption_key: int
    encrypted_main_block: bytes
    film_block: bytes
    offsets: dict[str, int]

    @classmethod
    def parse(cls, data: bytes) -> "SavContainer":
        reader = _Reader(data)
        header = tuple(reader.utf16z(name) for name in HEADER_FIELDS)
        if header[0] != "RSG":
            raise SavFormatError(f"signature: expected 'RSG', got {header[0]!r}")
        if header[7] != "EZ":
            raise SavFormatError(f"post_signature: expected 'EZ', got {header[7]!r}")

        offsets: dict[str, int] = {"header_end": reader.offset}
        preview_size = reader.u32("preview_block_size")
        offsets["preview_block"] = reader.offset
        preview_block = reader.take(preview_size, "preview_block")

        offsets["map_size"] = reader.offset
        map_size = reader.u32("map_block_size")
        offsets["map_block"] = reader.offset
        map_block = reader.take(map_size, "map_block")

        offsets["stored_crc32"] = reader.offset
        stored_crc32 = reader.u32("stored_crc32")
        offsets["encryption_key"] = reader.offset
        encryption_key = reader.u32("encryption_key")
        offsets["main_size"] = reader.offset
        main_size = reader.u32("main_block_size")
        offsets["main_block"] = reader.offset
        encrypted_main_block = reader.take(main_size, "encrypted_main_block")
        offsets["film_block"] = reader.offset
        film_block = reader.take(reader.remaining, "film_block")
        return cls(
            source_size=len(data),
            header=header,
            preview_block=preview_block,
            map_block=map_block,
            stored_crc32=stored_crc32,
            encryption_key=encryption_key,
            encrypted_main_block=encrypted_main_block,
            film_block=film_block,
            offsets=offsets,
        )

    def serialize(self) -> bytes:
        parts = [_utf16z(value) for value in self.header]
        parts.extend(
            (
                struct.pack("<I", len(self.preview_block)),
                self.preview_block,
                struct.pack("<I", len(self.map_block)),
                self.map_block,
                struct.pack("<I", self.stored_crc32),
                struct.pack("<I", self.encryption_key),
                struct.pack("<I", len(self.encrypted_main_block)),
                self.encrypted_main_block,
                self.film_block,
            )
        )
        return b"".join(parts)

    def main_payload(self) -> bytes:
        """Validate, decrypt and decompress the main model block."""

        decrypted = crypt_bytes(self.encrypted_main_block, self.encryption_key)
        calculated_crc32 = zlib.crc32(decrypted) & 0xFFFFFFFF
        if calculated_crc32 != self.stored_crc32:
            raise SavFormatError(
                f"main_block: CRC mismatch, stored 0x{self.stored_crc32:08X}, "
                f"calculated 0x{calculated_crc32:08X}"
            )
        return decompress_zl01(decrypted, "main_block").payload

    def with_main_payload(self, payload: bytes) -> "SavContainer":
        """Return a container with a rebuilt ZL01/CRC/XOR main block."""

        packed = compress_zl01(payload)
        return replace(
            self,
            stored_crc32=zlib.crc32(packed) & 0xFFFFFFFF,
            encrypted_main_block=crypt_bytes(packed, self.encryption_key),
        )

    def inner_envelope(self) -> InnerEnvelope:
        version = _parse_version(self.header[1])
        if version is None:
            raise SavFormatError(f"save version is not numeric: {self.header[1]!r}")
        return InnerEnvelope.parse(self.main_payload(), version)

    def galaxy_prefix(self) -> GalaxyPrefix:
        version = _parse_version(self.header[1])
        if version is None:
            raise SavFormatError(f"save version is not numeric: {self.header[1]!r}")
        payload = self.main_payload()
        envelope = InnerEnvelope.parse(payload, version)
        return GalaxyPrefix.parse(payload, envelope.galaxy_offset, version)

    def galaxy_directory(self) -> GalaxyDirectory:
        version = _parse_version(self.header[1])
        if version is None:
            raise SavFormatError(f"save version is not numeric: {self.header[1]!r}")
        payload = self.main_payload()
        envelope = InnerEnvelope.parse(payload, version)
        return GalaxyDirectory.parse(payload, envelope.galaxy_offset, version)

    def inspect(self, source_path: Path | None = None) -> dict[str, Any]:
        errors: list[str] = []

        def inspect_zl01(block: bytes, label: str) -> tuple[dict[str, Any], bytes | None]:
            if not block:
                return {"stored_size": 0, "present": False}, None
            result: dict[str, Any] = {
                "stored_size": len(block),
                "present": True,
                "sha256": _sha256(block),
            }
            try:
                unpacked = decompress_zl01(block, label)
                prefix_word_count = min(3, len(unpacked.payload) // 4)
                result.update(
                    {
                        "valid": True,
                        "unpacked_size": unpacked.actual_size,
                        "expected_unpacked_size": unpacked.expected_size,
                        "trailing_bytes": unpacked.trailing_bytes,
                        "unpacked_sha256": _sha256(unpacked.payload),
                        "prefix_u32": list(
                            struct.unpack(
                                "<" + "I" * prefix_word_count,
                                unpacked.payload[: prefix_word_count * 4],
                            )
                        ),
                    }
                )
                return result, unpacked.payload
            except SavFormatError as error:
                result.update({"valid": False, "error": str(error)})
                errors.append(str(error))
                return result, None

        preview, _ = inspect_zl01(self.preview_block, "preview_block")
        map_image, _ = inspect_zl01(self.map_block, "map_block")
        for image_report in (preview, map_image):
            prefix = image_report.get("prefix_u32")
            if prefix and len(prefix) == 3:
                width, height, row_stride = prefix
                expected_pixel_bytes = row_stride * height
                image_report["image_layout"] = {
                    "width": width,
                    "height": height,
                    "row_stride": row_stride,
                    "bytes_per_pixel": row_stride // width if width and row_stride % width == 0 else None,
                    "pixel_bytes": image_report["unpacked_size"] - 12,
                    "layout_size_valid": image_report["unpacked_size"] == expected_pixel_bytes + 12,
                }

        decrypted = crypt_bytes(self.encrypted_main_block, self.encryption_key)
        calculated_crc32 = zlib.crc32(decrypted) & 0xFFFFFFFF
        crc_valid = calculated_crc32 == self.stored_crc32
        if not crc_valid:
            errors.append(
                f"main_block: CRC mismatch, stored 0x{self.stored_crc32:08X}, "
                f"calculated 0x{calculated_crc32:08X}"
            )
        main, main_payload = inspect_zl01(decrypted, "main_block")
        main.update(
            {
                "encrypted_sha256": _sha256(self.encrypted_main_block),
                "decrypted_sha256": _sha256(decrypted),
                "stored_crc32": f"0x{self.stored_crc32:08X}",
                "calculated_crc32": f"0x{calculated_crc32:08X}",
                "crc_valid": crc_valid,
                "encryption_key": self.encryption_key,
            }
        )
        if main_payload is not None and len(main_payload) >= INNER_METADATA_SIZE:
            try:
                main["metadata_prefix"] = InnerMetadata.parse(main_payload).as_dict()
                main["metadata_prefix"]["validated"] = True
            except SavFormatError as error:
                main["metadata_prefix"] = {
                    "validated": False,
                    "error": str(error),
                    "raw_hex": main_payload[:INNER_METADATA_SIZE].hex().upper(),
                }
            try:
                version = _parse_version(self.header[1])
                if version is None:
                    raise SavFormatError(
                        f"main_block: save version is not numeric: {self.header[1]!r}"
                    )
                envelope = InnerEnvelope.parse(main_payload, version)
                main["inner_envelope"] = envelope.as_dict()
                main["galaxy_prefix"] = GalaxyPrefix.parse(
                    main_payload, envelope.galaxy_offset, version
                ).as_dict()
                main["galaxy_directory"] = GalaxyDirectory.parse(
                    main_payload, envelope.galaxy_offset, version
                ).as_dict()
            except SavFormatError as error:
                main["inner_envelope"] = {
                    "validated": False,
                    "error": str(error),
                }

        serialized = self.serialize()
        roundtrip_exact = len(serialized) == self.source_size
        result: dict[str, Any] = {
            "schema": "srhd-save-inspect-v1",
            "source": str(source_path.resolve()) if source_path else None,
            "source_size": self.source_size,
            "source_sha256": _sha256(serialized),
            "header": dict(zip(HEADER_FIELDS, self.header)),
            "version": _parse_version(self.header[1]),
            "offsets": {name: f"0x{offset:X}" for name, offset in self.offsets.items()},
            "preview_block": preview,
            "map_block": map_image,
            "main_block": main,
            "film_block": {
                "size": len(self.film_block),
                "sha256": _sha256(self.film_block),
            },
            "lossless_container_roundtrip": roundtrip_exact,
            "valid": not errors and roundtrip_exact,
            "errors": errors,
        }
        return result


def _parse_version(value: str) -> int | None:
    digits = "".join(character for character in value if character.isdecimal())
    return int(digits) if digits else None


def inspect_path(path: Path) -> tuple[SavContainer, dict[str, Any]]:
    data = path.read_bytes()
    container = SavContainer.parse(data)
    report = container.inspect(path)
    report["source_sha256"] = _sha256(data)
    report["lossless_container_roundtrip"] = container.serialize() == data
    report["valid"] = report["valid"] and report["lossless_container_roundtrip"]
    if not report["lossless_container_roundtrip"]:
        report["errors"].append("lossless container serialization differs from input")
    return container, report


def _checked_i32(value: int, name: str) -> int:
    if not -(1 << 31) <= value < (1 << 31):
        raise SavFormatError(f"{name}: {value} is outside signed Int32 range")
    return value


def _checked_u32(value: int, name: str) -> int:
    if not 0 <= value <= 0xFFFFFFFF:
        raise SavFormatError(f"{name}: {value} is outside UInt32 range")
    return value


def patch_inner_metadata(payload: bytes, changes: dict[str, Any]) -> tuple[bytes, dict[str, Any]]:
    """Patch only proven metadata fields and preserve every other payload byte."""

    before = InnerMetadata.parse(payload)
    unknown = sorted(set(changes) - set(INNER_METADATA_FIELD_RANGES))
    if unknown:
        raise SavFormatError(f"unsupported metadata field(s): {', '.join(unknown)}")

    patched = bytearray(payload)
    if "camera_x" in changes:
        struct.pack_into("<i", patched, 4, _checked_i32(int(changes["camera_x"]), "camera_x"))
    if "camera_y" in changes:
        struct.pack_into("<i", patched, 8, _checked_i32(int(changes["camera_y"]), "camera_y"))
    if "show_panel" in changes:
        patched[12] = int(bool(changes["show_panel"]))
    if "view_follow" in changes:
        patched[14] = int(bool(changes["view_follow"]))
    if "calc_header" in changes:
        patched[19] = int(bool(changes["calc_header"]))
    if "tips" in changes:
        struct.pack_into("<I", patched, 20, _checked_u32(int(changes["tips"]), "tips"))

    allowed_offsets = {
        offset
        for field in changes
        for offset in INNER_METADATA_FIELD_RANGES[field]
    }
    changed_offsets = [
        offset for offset, (old, new) in enumerate(zip(payload, patched)) if old != new
    ]
    unexpected = sorted(set(changed_offsets) - allowed_offsets)
    if unexpected:
        raise AssertionError(f"internal patch escaped allowed offsets: {unexpected}")

    after = InnerMetadata.parse(bytes(patched))
    changed_fields = {
        field: {"before": getattr(before, field), "after": getattr(after, field)}
        for field in INNER_METADATA_FIELD_RANGES
        if getattr(before, field) != getattr(after, field)
    }
    return bytes(patched), {
        "before": before.as_dict(),
        "after": after.as_dict(),
        "changed_fields": changed_fields,
        "changed_unpacked_offsets": [f"0x{offset:X}" for offset in changed_offsets],
        "opaque_bytes_preserved": all(
            payload[offset] == patched[offset]
            for offset in range(len(payload))
            if offset not in allowed_offsets
        ),
    }


def write_metadata_patch(
    input_path: Path,
    output_path: Path,
    changes: dict[str, Any],
) -> dict[str, Any]:
    """Write and independently re-read a patched copy; never overwrite a file."""

    container, report = inspect_path(input_path)
    if not report["valid"]:
        raise SavFormatError("input failed validation; refusing to write patched output")
    version = report["version"]
    if version not in SUPPORTED_PATCH_VERSIONS:
        raise SavFormatError(
            f"metadata patching is regression-tested only for versions "
            f"{sorted(SUPPORTED_PATCH_VERSIONS)}, got {version!r}"
        )
    if not changes:
        raise SavFormatError("no metadata changes were requested")

    original_data = input_path.read_bytes()
    original_payload = container.main_payload()
    patched_payload, patch_report = patch_inner_metadata(original_payload, changes)
    patched_container = (
        container
        if patched_payload == original_payload
        else container.with_main_payload(patched_payload)
    )

    output_path.parent.mkdir(parents=True, exist_ok=True)
    with output_path.open("xb") as stream:
        stream.write(patched_container.serialize())

    output_container, output_report = inspect_path(output_path)
    output_payload = output_container.main_payload()
    preserved_components = {
        "header": output_container.header == container.header,
        "preview_block": output_container.preview_block == container.preview_block,
        "map_block": output_container.map_block == container.map_block,
        "encryption_key": output_container.encryption_key == container.encryption_key,
        "film_block": output_container.film_block == container.film_block,
    }
    verified = (
        output_report["valid"]
        and output_payload == patched_payload
        and patch_report["opaque_bytes_preserved"]
        and all(preserved_components.values())
    )
    result = {
        "schema": "srhd-save-metadata-patch-v1",
        "input": str(input_path.resolve()),
        "output": str(output_path.resolve()),
        "input_sha256": _sha256(original_data),
        "output_sha256": _sha256(output_path.read_bytes()),
        "version": version,
        "patch": patch_report,
        "preserved_components": preserved_components,
        "opaque_model_offset": f"0x{INNER_METADATA_SIZE:X}",
        "opaque_model_bytes_preserved": original_payload[INNER_METADATA_SIZE:] == output_payload[INNER_METADATA_SIZE:],
        "output_valid": output_report["valid"],
        "verified": verified,
    }
    if not verified:
        raise SavFormatError("post-write verification failed; keep the original SAV")
    return result


def _emit_json(payload: Any, output: Path | None) -> None:
    rendered = json.dumps(payload, ensure_ascii=False, indent=2) + "\n"
    if output is None:
        print(rendered, end="")
        return
    output.parent.mkdir(parents=True, exist_ok=True)
    with output.open("x", encoding="utf-8", newline="\n") as stream:
        stream.write(rendered)


def _command_inspect(args: argparse.Namespace) -> int:
    _, report = inspect_path(args.input)
    _emit_json(report, args.output)
    return 0 if report["valid"] else 2


def _command_verify(args: argparse.Namespace) -> int:
    reports: list[dict[str, Any]] = []
    exit_code = 0
    for path in args.inputs:
        try:
            _, report = inspect_path(path)
        except (OSError, SavFormatError) as error:
            report = {
                "schema": "srhd-save-inspect-v1",
                "source": str(path.resolve()),
                "valid": False,
                "errors": [str(error)],
            }
        reports.append(report)
        if not report["valid"]:
            exit_code = 2
    _emit_json(reports, args.output)
    return exit_code


def _command_roundtrip(args: argparse.Namespace) -> int:
    container, report = inspect_path(args.input)
    if not report["valid"]:
        raise SavFormatError("input failed validation; refusing to write round-trip output")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    with args.output.open("xb") as stream:
        stream.write(container.serialize())
    output_data = args.output.read_bytes()
    result = {
        "schema": "srhd-save-roundtrip-v1",
        "input": str(args.input.resolve()),
        "output": str(args.output.resolve()),
        "size": len(output_data),
        "sha256": _sha256(output_data),
        "byte_identical": output_data == args.input.read_bytes(),
    }
    print(json.dumps(result, ensure_ascii=False, indent=2))
    return 0 if result["byte_identical"] else 2


def _command_patch_metadata(args: argparse.Namespace) -> int:
    changes = {
        name: value
        for name, value in (
            ("camera_x", args.camera_x),
            ("camera_y", args.camera_y),
            ("show_panel", args.show_panel),
            ("view_follow", args.view_follow),
            ("calc_header", args.calc_header),
            ("tips", args.tips),
        )
        if value is not None
    }
    result = write_metadata_patch(args.input, args.output, changes)
    print(json.dumps(result, ensure_ascii=False, indent=2))
    return 0 if result["verified"] else 2


def _bool_argument(value: str) -> bool:
    normalized = value.casefold()
    if normalized in ("1", "true", "yes", "on", "да"):
        return True
    if normalized in ("0", "false", "no", "off", "нет"):
        return False
    raise argparse.ArgumentTypeError(f"expected true/false, got {value!r}")


def _integer_argument(value: str) -> int:
    try:
        return int(value, 0)
    except ValueError as error:
        raise argparse.ArgumentTypeError(f"expected integer, got {value!r}") from error


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Inspect and validate a Space Rangers HD SAV container"
    )
    subparsers = parser.add_subparsers(dest="command", required=True)

    inspect_parser = subparsers.add_parser("inspect", help="inspect one SAV as JSON")
    inspect_parser.add_argument("input", type=Path)
    inspect_parser.add_argument("--output", type=Path, help="write a new JSON report; never overwrites")
    inspect_parser.set_defaults(handler=_command_inspect)

    verify_parser = subparsers.add_parser("verify", help="validate one or more SAV files")
    verify_parser.add_argument("inputs", nargs="+", type=Path)
    verify_parser.add_argument("--output", type=Path, help="write a new JSON report; never overwrites")
    verify_parser.set_defaults(handler=_command_verify)

    roundtrip_parser = subparsers.add_parser(
        "roundtrip", help="write a new byte-identical container; never overwrites"
    )
    roundtrip_parser.add_argument("input", type=Path)
    roundtrip_parser.add_argument("output", type=Path)
    roundtrip_parser.set_defaults(handler=_command_roundtrip)

    patch_parser = subparsers.add_parser(
        "patch-metadata",
        help="patch only proven UI metadata and write a verified new SAV",
    )
    patch_parser.add_argument("input", type=Path)
    patch_parser.add_argument("output", type=Path)
    patch_parser.add_argument("--camera-x", type=_integer_argument)
    patch_parser.add_argument("--camera-y", type=_integer_argument)
    patch_parser.add_argument("--show-panel", type=_bool_argument)
    patch_parser.add_argument("--view-follow", type=_bool_argument)
    patch_parser.add_argument("--calc-header", type=_bool_argument)
    patch_parser.add_argument("--tips", type=_integer_argument, help="UInt32, decimal or 0x-prefixed")
    patch_parser.set_defaults(handler=_command_patch_metadata)
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)
    try:
        return int(args.handler(args))
    except (OSError, SavFormatError) as error:
        print(f"error: {error}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())

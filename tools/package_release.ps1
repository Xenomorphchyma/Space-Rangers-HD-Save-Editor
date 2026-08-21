[CmdletBinding()]
param(
    [string]$Version = '1.0.0-rc.1'
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$releaseRoot = Join-Path $projectRoot 'release'
$exe = Join-Path $releaseRoot 'SRHDSaveEditor.exe'
$manifestPath = Join-Path $releaseRoot 'manifest.json'

if (-not (Test-Path -LiteralPath $exe)) { throw "Executable not found: $exe" }
if (-not (Test-Path -LiteralPath $manifestPath)) { throw "Manifest not found: $manifestPath" }

$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$actualHash = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
if ($manifest.sha256 -ne $actualHash) {
    throw "Manifest SHA-256 does not match SRHDSaveEditor.exe. Run tools\verify.ps1 first."
}

$archive = Join-Path $releaseRoot ("Space-Rangers-HD-Save-Editor-{0}-win-x86.zip" -f $Version)
$files = @(
    $exe,
    (Join-Path $projectRoot 'README.md'),
    (Join-Path $projectRoot 'LICENSE')
)
foreach ($file in $files) {
    if (-not (Test-Path -LiteralPath $file)) { throw "Release file not found: $file" }
}

Compress-Archive -LiteralPath $files -DestinationPath $archive -CompressionLevel Optimal -Force
$archiveItem = Get-Item -LiteralPath $archive
$archiveHash = Get-FileHash -LiteralPath $archive -Algorithm SHA256
[pscustomobject]@{
    Path = $archiveItem.FullName
    Length = $archiveItem.Length
    SHA256 = $archiveHash.Hash
    ExecutableSHA256 = $actualHash
}

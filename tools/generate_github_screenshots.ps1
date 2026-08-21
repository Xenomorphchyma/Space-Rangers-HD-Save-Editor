[CmdletBinding()]
param(
    [string]$SavePath = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectRoot = Split-Path -Parent $PSScriptRoot
$buildRoot = Join-Path $projectRoot 'build'
$outputRoot = Join-Path $projectRoot 'docs\screenshots'
$generator = Join-Path $buildRoot 'GitHubScreenshotGenerator.exe'
$csc = 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'

if ([string]::IsNullOrWhiteSpace($SavePath)) {
    $SavePath = Join-Path ([Environment]::GetFolderPath('MyDocuments')) `
        'SpaceRangersHD\Save\AutoSave.sav'
}

if (-not (Test-Path -LiteralPath $SavePath -PathType Leaf)) {
    throw "SAV fixture not found: $SavePath"
}

New-Item -ItemType Directory -Force -Path $buildRoot, $outputRoot | Out-Null

$references = @(
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Drawing.dll',
    '/reference:System.IO.Compression.dll',
    '/reference:System.Windows.Forms.dll'
)
$sources = @(
    (Join-Path $projectRoot 'src\Program.cs'),
    (Join-Path $projectRoot 'src\AppSettings.cs'),
    (Join-Path $projectRoot 'src\SavContainer.cs'),
    (Join-Path $projectRoot 'src\AchievementCatalog.cs'),
    (Join-Path $projectRoot 'src\GameDataCatalog.cs'),
    (Join-Path $projectRoot 'src\MainForm.cs'),
    (Join-Path $projectRoot 'src\UI\EditorAssets.cs'),
    (Join-Path $projectRoot 'src\UI\EditorLocalization.cs'),
    (Join-Path $projectRoot 'src\UI\RussianCaptions.cs'),
    (Join-Path $projectRoot 'src\UI\EditorFormDefinitions.cs'),
    (Join-Path $projectRoot 'src\UI\EditorFormFactory.cs'),
    (Join-Path $projectRoot 'tests\GitHubScreenshotGenerator.cs')
)

& $csc /nologo /target:exe /main:GitHubScreenshotGenerator /platform:x86 /optimize+ /utf8output `
    "/out:$generator" $references $sources
if ($LASTEXITCODE -ne 0) { throw 'GitHub screenshot generator compilation failed.' }

& $generator ru $SavePath (Join-Path $outputRoot 'main-ru.png')
if ($LASTEXITCODE -ne 0) { throw 'Russian GitHub screenshot generation failed.' }
& $generator en $SavePath (Join-Path $outputRoot 'main-en.png')
if ($LASTEXITCODE -ne 0) { throw 'English GitHub screenshot generation failed.' }

Get-ChildItem -LiteralPath $outputRoot -Filter 'main-*.png' |
    Select-Object FullName, Length, LastWriteTime

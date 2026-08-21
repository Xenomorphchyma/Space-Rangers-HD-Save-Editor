[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectRoot = Split-Path -Parent $PSScriptRoot
$buildRoot = Join-Path $projectRoot 'build'
$releaseRoot = Join-Path $projectRoot 'release'
$output = Join-Path $releaseRoot 'SRHDSaveEditor.exe'
$icon = Join-Path $buildRoot 'SRHDSaveEditor.ico'
$csc = 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $csc)) {
    throw "Не найден компилятор .NET Framework: $csc"
}

New-Item -ItemType Directory -Force -Path $buildRoot, $releaseRoot | Out-Null

$references = @(
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Drawing.dll',
    '/reference:System.IO.Compression.dll',
    '/reference:System.Windows.Forms.dll'
)

$iconGenerator = Join-Path $buildRoot 'IconGenerator.exe'
& $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
    "/out:$iconGenerator" $references `
    (Join-Path $projectRoot 'src\UI\EditorAssets.cs') `
    (Join-Path $projectRoot 'tools\IconGenerator.cs')
if ($LASTEXITCODE -ne 0) { throw 'Не удалось собрать генератор собственной иконки.' }
& $iconGenerator $icon
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $icon)) {
    throw 'Не удалось создать собственную иконку.'
}

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
    (Join-Path $projectRoot 'src\UI\EditorFormFactory.cs')
)

if (Test-Path -LiteralPath $output) { [IO.File]::Delete($output) }
& $csc /nologo /target:winexe /platform:x86 /optimize+ /debug- /utf8output `
    "/win32icon:$icon" "/out:$output" $references $sources
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $output)) {
    throw 'Сборка Space Rangers HD Save Editor завершилась с ошибкой.'
}

$item = Get-Item -LiteralPath $output
$hash = Get-FileHash -LiteralPath $output -Algorithm SHA256
[pscustomobject]@{
    Path = $item.FullName
    Length = $item.Length
    SHA256 = $hash.Hash
    Architecture = 'x86'
    Runtime = '.NET Framework 4.8'
} | Format-List

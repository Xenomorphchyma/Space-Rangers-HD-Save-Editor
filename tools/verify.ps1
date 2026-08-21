[CmdletBinding()]
param(
    [string]$CorpusList,
    [string]$SupplementalCorpusList,
    [string]$GamePath = '',
    [string]$Version = '1.0.0-rc.1'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectRoot = Split-Path -Parent $PSScriptRoot
$buildRoot = Join-Path $projectRoot 'build'
$releaseExe = Join-Path $projectRoot 'release\SRHDSaveEditor.exe'
$csc = 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'
$references = @(
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Drawing.dll',
    '/reference:System.IO.Compression.dll',
    '/reference:System.Windows.Forms.dll'
)

& (Join-Path $PSScriptRoot 'build.ps1')

Push-Location $projectRoot
try {
    python -m unittest discover -s tests -p 'test_*.py' -v
    if ($LASTEXITCODE -ne 0) { throw 'Python tests failed.' }

    $appSources = @(
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
    $formsTest = Join-Path $buildRoot 'EditorFormsSelfTest.exe'
    & $csc /nologo /target:exe /main:EditorFormsSelfTest /platform:x86 /optimize+ /utf8output `
        "/out:$formsTest" $references $appSources `
        (Join-Path $projectRoot 'tests\EditorFormsSelfTest.cs')
    if ($LASTEXITCODE -ne 0) { throw 'UI semantic self-test compilation failed.' }
    & $formsTest
    if ($LASTEXITCODE -ne 0) { throw 'UI semantic self-test failed.' }

    $localizationTest = Join-Path $buildRoot 'EditorLocalizationSelfTest.exe'
    & $csc /nologo /target:exe /main:EditorLocalizationSelfTest /platform:x86 /optimize+ /utf8output `
        "/out:$localizationTest" $references $appSources `
        (Join-Path $projectRoot 'tests\EditorLocalizationSelfTest.cs')
    if ($LASTEXITCODE -ne 0) { throw 'UI localization self-test compilation failed.' }
    & $localizationTest
    if ($LASTEXITCODE -ne 0) { throw 'UI localization self-test failed.' }

    $visualTest = Join-Path $buildRoot 'MainWindowVisualSmokeTest.exe'
    & $csc /nologo /target:exe /main:MainWindowVisualSmokeTest /platform:x86 /optimize+ /utf8output `
        "/out:$visualTest" $references $appSources `
        (Join-Path $projectRoot 'tests\MainWindowVisualSmokeTest.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Main-window visual smoke compilation failed.' }

    $dialogVisualTest = Join-Path $buildRoot 'EditorDialogsVisualSmokeTest.exe'
    & $csc /nologo /target:exe /main:EditorDialogsVisualSmokeTest /platform:x86 /optimize+ /utf8output `
        "/out:$dialogVisualTest" $references $appSources `
        (Join-Path $projectRoot 'tests\EditorDialogsVisualSmokeTest.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Editor-dialog visual smoke compilation failed.' }
    & $dialogVisualTest (Join-Path $buildRoot 'editor-dialogs-smoke.png')
    if ($LASTEXITCODE -ne 0) { throw 'Editor-dialog visual smoke failed.' }

    $core = Join-Path $projectRoot 'src\SavContainer.cs'
    $corpusTest = Join-Path $buildRoot 'CorpusSelfTest.exe'
    & $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
        "/out:$corpusTest" $references $core `
        (Join-Path $projectRoot 'tests\CorpusSelfTest.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Corpus self-test compilation failed.' }

    $hullTest = Join-Path $buildRoot 'HullInterceptorRoundtrip.exe'
    & $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
        "/out:$hullTest" $references $core `
        (Join-Path $projectRoot 'tests\HullInterceptorRoundtrip.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Hull round-trip compilation failed.' }

    $weaponTest = Join-Path $buildRoot 'CustomWeaponRenameRoundtrip.exe'
    & $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
        "/out:$weaponTest" $references $core `
        (Join-Path $projectRoot 'tests\CustomWeaponRenameRoundtrip.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Custom weapon round-trip compilation failed.' }

    $corpusVerified = 0
    $visualVerified = $false
    $catalogReferenceVerified = $false
    $fixture = $null
    if (-not [string]::IsNullOrWhiteSpace($CorpusList)) {
        $resolvedCorpus = [IO.Path]::GetFullPath($CorpusList)
        if (-not (Test-Path -LiteralPath $resolvedCorpus)) { throw "Corpus list not found: $resolvedCorpus" }
        $paths = @(Get-Content -LiteralPath $resolvedCorpus | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
        if ($paths.Count -eq 0) { throw 'Corpus list is empty.' }
        foreach ($path in $paths) {
            if (-not (Test-Path -LiteralPath $path)) { throw "Corpus save not found: $path" }
        }
        $fixture = $paths | Where-Object { [IO.Path]::GetFileName($_) -eq 'AutoSave.sav' } | Select-Object -First 1
        if (-not $fixture) { $fixture = $paths[0] }
        $stamp = [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
        & $hullTest $fixture (Join-Path $buildRoot "hull-$stamp.sav")
        if ($LASTEXITCODE -ne 0) { throw 'Hull structural round-trip failed.' }
        & $weaponTest $fixture (Join-Path $buildRoot "weapon-$stamp.sav")
        if ($LASTEXITCODE -ne 0) { throw 'Custom weapon structural round-trip failed.' }
        & $corpusTest --list $resolvedCorpus (Join-Path $buildRoot "roundtrip-$stamp.sav") (Join-Path $buildRoot "patched-$stamp.sav")
        if ($LASTEXITCODE -ne 0) { throw 'Full SAV corpus test failed.' }
        $corpusVerified = $paths.Count
        if (-not [string]::IsNullOrWhiteSpace($SupplementalCorpusList)) {
            $resolvedSupplemental = [IO.Path]::GetFullPath($SupplementalCorpusList)
            if (-not (Test-Path -LiteralPath $resolvedSupplemental)) {
                throw "Supplemental corpus list not found: $resolvedSupplemental"
            }
            $supplementalPaths = @(Get-Content -LiteralPath $resolvedSupplemental |
                Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
            if ($supplementalPaths.Count -eq 0) { throw 'Supplemental corpus list is empty.' }
            foreach ($path in $supplementalPaths) {
                if (-not (Test-Path -LiteralPath $path)) {
                    throw "Supplemental corpus save not found: $path"
                }
            }
            & $corpusTest --list $resolvedSupplemental
            if ($LASTEXITCODE -ne 0) { throw 'Supplemental SAV structural test failed.' }
            $corpusVerified += $supplementalPaths.Count
        }
        & $visualTest $fixture (Join-Path $buildRoot "main-window-$stamp.png")
        if ($LASTEXITCODE -ne 0) { throw 'Main-window visual smoke failed.' }
        $visualVerified = $true
    }

    $catalogVerified = $false
    if (-not [string]::IsNullOrWhiteSpace($GamePath) -and
        (Test-Path -LiteralPath (Join-Path $GamePath 'CFG\Main.dat'))) {
        $catalogTest = Join-Path $buildRoot 'CatalogSelfTest.exe'
        & $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
            "/out:$catalogTest" $references `
            (Join-Path $projectRoot 'src\GameDataCatalog.cs') `
            (Join-Path $projectRoot 'tests\CatalogSelfTest.cs')
        if ($LASTEXITCODE -ne 0) { throw 'Catalog self-test compilation failed.' }
        & $catalogTest $GamePath
        if ($LASTEXITCODE -ne 0) { throw 'Catalog self-test failed.' }
        $catalogVerified = $true

        if ($null -ne $fixture) {
            $catalogReferenceTest = Join-Path $buildRoot 'CatalogReferenceSelfTest.exe'
            & $csc /nologo /target:exe /platform:anycpu /optimize+ /utf8output `
                "/out:$catalogReferenceTest" $references `
                (Join-Path $projectRoot 'src\SavContainer.cs') `
                (Join-Path $projectRoot 'src\GameDataCatalog.cs') `
                (Join-Path $projectRoot 'tests\CatalogReferenceSelfTest.cs')
            if ($LASTEXITCODE -ne 0) { throw 'Catalog-reference self-test compilation failed.' }
            $catalogCandidates = @($paths | Sort-Object {
                (Get-Item -LiteralPath $_).Length
            })
            $catalogAttemptLimit = [Math]::Min(16, $catalogCandidates.Count)
            $lastCatalogOutput = @()
            for ($catalogAttempt = 0; $catalogAttempt -lt $catalogAttemptLimit; $catalogAttempt++) {
                $candidate = $catalogCandidates[$catalogAttempt]
                $lastCatalogOutput = @(& $catalogReferenceTest $GamePath $candidate 2>&1)
                if ($LASTEXITCODE -eq 0) {
                    $lastCatalogOutput | Write-Output
                    Write-Output ("catalog-reference fixture: " + $candidate)
                    $catalogReferenceVerified = $true
                    break
                }
            }
            if (-not $catalogReferenceVerified) {
                $lastCatalogOutput | Write-Output
                throw "Catalog-reference self-test failed for the first $catalogAttemptLimit size-sorted corpus fixtures."
            }
        }
    }

    $item = Get-Item -LiteralPath $releaseExe
    $hash = Get-FileHash -LiteralPath $releaseExe -Algorithm SHA256
    $manifest = [ordered]@{
        schema = 'srhd-save-editor-release-v1'
        product = 'Space Rangers HD Save Editor'
        author = 'Xenomorphchyma'
        repository = 'https://github.com/Xenomorphchyma/Space-Rangers-HD-Save-Editor'
        version = $Version
        file = 'SRHDSaveEditor.exe'
        length = $item.Length
        sha256 = $hash.Hash
        architecture = 'x86'
        runtime = '.NET Framework 4.8'
        tests = [ordered]@{
            python = 8
            semantic_forms = 48
            semantic_controls = 1958
            editor_dialog_visual_smoke = $true
            visual_smoke = $visualVerified
            corpus_saves = $corpusVerified
            game_catalog = $catalogVerified
            catalog_references = $catalogReferenceVerified
        }
        privacy = [ordered]@{
            network_code = $false
            telemetry = $false
            original_save_overwrite = $false
        }
    }
    $manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $projectRoot 'release\manifest.json') -Encoding UTF8
    $manifest | ConvertTo-Json -Depth 5
}
finally {
    Pop-Location
}

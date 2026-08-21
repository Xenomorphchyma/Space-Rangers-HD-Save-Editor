[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$DfmDirectory,
    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
$entries = [System.Collections.Generic.SortedDictionary[string,string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase)

foreach ($file in Get-ChildItem -LiteralPath $DfmDirectory -Filter '*.dfm' -File | Sort-Object Name) {
    $resource = [IO.Path]::GetFileNameWithoutExtension($file.Name)
    $stack = [System.Collections.Generic.List[string]]::new()
    $indentStack = [System.Collections.Generic.List[int]]::new()
    $allNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $captionNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($line in Get-Content -LiteralPath $file.FullName -Encoding UTF8) {
        $indent = $line.Length - $line.TrimStart().Length
        if ($line -match '^\s*(?:object|inherited|inline)\s+([^:]+):') {
            $name = $Matches[1].Trim()
            while ($indentStack.Count -gt 0 -and $indentStack[$indentStack.Count - 1] -ge $indent) {
                $stack.RemoveAt($stack.Count - 1)
                $indentStack.RemoveAt($indentStack.Count - 1)
            }
            $stack.Add($name)
            $indentStack.Add($indent)
            [void]$allNames.Add($name)
            continue
        }
        if ($stack.Count -eq 0 -or $line -notmatch "^\s*Caption\s*=\s*'(.*)'\s*$") { continue }
        $caption = $Matches[1].Replace("''", "'")
        $caption = [regex]::Replace($caption, "'(?:#13|#10)+'", "`r`n")
        $name = $stack[$stack.Count - 1]
        [void]$captionNames.Add($name)
        if ($caption -eq $name) { $entries["$resource/$name"] = ''; continue }
        $entries["$resource/$name"] = $caption
    }
    foreach ($name in $allNames) {
        if (-not $captionNames.Contains($name)) { $entries["$resource/$name"] = '' }
    }
}

function Escape-CSharp([string]$value) {
    return $value.Replace('\', '\\').Replace('"', '\"').Replace("`r", '\r').Replace("`n", '\n')
}

$body = [System.Collections.Generic.List[string]]::new()
$body.Add('// Russian UI caption compatibility table. See NOTICE.md.')
$body.Add('using System;')
$body.Add('using System.Collections.Generic;')
$body.Add('')
$body.Add('namespace SpaceRangersHdSaveEditor')
$body.Add('{')
$body.Add('    internal static class RussianCaptions')
$body.Add('    {')
$body.Add('        private static readonly Dictionary<string, string> Values =')
$body.Add('            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)')
$body.Add('            {')
foreach ($entry in $entries.GetEnumerator()) {
    $key = Escape-CSharp $entry.Key
    $value = Escape-CSharp $entry.Value
    $body.Add("                { `"$key`", `"$value`" },")
}
$body.Add('            };')
$body.Add('')
$body.Add('        internal static bool TryGet(string resource, string controlName, out string caption)')
$body.Add('        {')
$body.Add('            return Values.TryGetValue((resource ?? string.Empty) + "/" +')
$body.Add('                (controlName ?? string.Empty), out caption);')
$body.Add('        }')
$body.Add('    }')
$body.Add('}')

$directory = Split-Path -Parent $OutputPath
if ($directory) { New-Item -ItemType Directory -Force -Path $directory | Out-Null }
[IO.File]::WriteAllLines($OutputPath, $body, [Text.UTF8Encoding]::new($false))
Write-Host "Generated $($entries.Count) Russian captions: $OutputPath"

param(
    [string]$GamePath = 'E:/Games/Vintagestory',
    [string]$RuntimeReferencePath = '',
    [switch]$SkipTests
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
Set-Location -LiteralPath $root
if (-not (Test-Path -LiteralPath (Join-Path $GamePath 'VintagestoryAPI.dll'))) { throw 'Set -GamePath to your Vintage Story 1.22.7 installation.' }
if (-not $RuntimeReferencePath) {
    $runtimeRoot = Join-Path ${env:ProgramFiles} 'dotnet/shared/Microsoft.NETCore.App'
    $runtime = Get-ChildItem -LiteralPath $runtimeRoot -Directory | Where-Object { $_.Name -match '^10\.\d+\.\d+$' } | Sort-Object { [version]$_.Name } -Descending | Select-Object -First 1
    if (-not $runtime) { throw 'A .NET 10 runtime is required for the installed Vintage Story API.' }
    $RuntimeReferencePath = $runtime.FullName
}
$env:DOTNET_CLI_HOME = Join-Path $root '.dotnet'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$env:MSBuildEnableWorkloadResolver = 'false'
if (-not $SkipTests) {
    & dotnet run --project 'tests/Procession.Core.Tests' --configuration Release
    if ($LASTEXITCODE -ne 0) { throw 'Behavioral tests failed.' }
    & (Join-Path $PSScriptRoot 'Test-Assets.ps1') -GamePath $GamePath
}
$build = Join-Path $root 'artifacts/build'
& dotnet build 'src/Procession.Game/Procession.Game.csproj' --configuration Release --output $build "-p:VintageStoryPath=$GamePath" "-p:RuntimeReferencePath=$RuntimeReferencePath" --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw 'Game integration build failed.' }
$metadata = Get-Content -LiteralPath (Join-Path $root 'modinfo.json') -Raw | ConvertFrom-Json
$dist = Join-Path $root 'dist'
[IO.Directory]::CreateDirectory($dist) | Out-Null
$zipPath = Join-Path $dist "$($metadata.modid)-$($metadata.version).zip"
# A fresh staging directory prevents obsolete assets from leaking into a new package.
$stage = Join-Path $root ('artifacts/package-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($stage) | Out-Null
foreach ($dll in @('Procession.dll', 'Procession.Core.dll')) { Copy-Item -LiteralPath (Join-Path $build $dll) -Destination $stage }
Copy-Item -LiteralPath (Join-Path $root 'modinfo.json') -Destination $stage
[IO.Directory]::CreateDirectory((Join-Path $stage 'assets')) | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'assets/procession') -Destination (Join-Path $stage 'assets') -Recurse
if (Test-Path -LiteralPath (Join-Path $root 'docs/PLAYTEST.md')) { Copy-Item -LiteralPath (Join-Path $root 'docs/PLAYTEST.md') -Destination (Join-Path $stage 'PLAYTEST.md') }
@'
The Procession 0.1.0 — development alpha for Vintage Story 1.22.7
Install the ZIP in a separate test profile's Mods folder; required on server and clients.
Use /procession guide or a craftable Route Chart to investigate the relay stations.
This development build needs the in-game checks listed in PLAYTEST.md. It is not a balanced release.
No game binaries or textures are redistributed. Original mod content is under the project's current rights; no open-source license has been selected.
'@ | Set-Content -LiteralPath (Join-Path $stage 'README.txt') -Encoding utf8
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zipPath -CompressionLevel Optimal -Force
$manifest = Get-ChildItem -LiteralPath $stage -Recurse -File | Sort-Object FullName | ForEach-Object {
    [ordered]@{ path = [IO.Path]::GetRelativePath($stage, $_.FullName).Replace('\','/'); bytes = $_.Length; sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $dist "$($metadata.modid)-$($metadata.version).manifest.json") -Encoding utf8
Write-Output "Built $zipPath"
Write-Output "SHA256 $((Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash)"
Write-Output 'Package created locally. No mod was installed into the game and no existing world was changed.'

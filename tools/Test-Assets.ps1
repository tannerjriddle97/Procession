param([string]$GamePath = 'E:/Games/Vintagestory')
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$assetRoot = Join-Path $root 'assets/procession'
$script:checks = 0
function Assert-Asset($condition, $message) {
    if (-not $condition) { throw $message }
    $script:checks++
}
function Check-Reference([string]$code, [string]$folder, [string]$extension) {
    if ([string]::IsNullOrWhiteSpace($code)) { throw "Empty $folder reference" }
    $parts = $code -split ':', 2
    if ($parts.Count -eq 2) { $domain = $parts[0]; $path = $parts[1] } else { $domain = 'procession'; $path = $code }
    if ($domain -eq 'procession') {
        Assert-Asset (Test-Path -LiteralPath (Join-Path $assetRoot "$folder/$path$extension")) "Unresolved asset: $code ($folder)"
    } elseif ($domain -eq 'game' -and (Test-Path -LiteralPath $GamePath)) {
        $found = @('survival','game','creative') | Where-Object { Test-Path -LiteralPath (Join-Path $GamePath "assets/$_/$folder/$path$extension") }
        Assert-Asset ($found.Count -gt 0) "Unresolved vanilla asset: $code ($folder)"
    } else { throw "Unknown asset namespace: $code" }
}
$jsonFiles = Get-ChildItem -LiteralPath $assetRoot -Recurse -File -Filter '*.json'
foreach ($file in $jsonFiles) {
    $data = Get-Content -LiteralPath $file.FullName -Raw | ConvertFrom-Json -AsHashtable
    Assert-Asset ($null -ne $data) "Invalid JSON: $($file.FullName)"
    $relative = [IO.Path]::GetRelativePath($assetRoot, $file.FullName).Replace('\','/')
    if ($relative.StartsWith('shapes/')) {
        $names = [Collections.Generic.HashSet[string]]::new()
        function Check-Element($element) {
            Assert-Asset ($names.Add($element.name)) "Duplicate shape element $($element.name) in $relative"
            Assert-Asset ($element.from.Count -eq 3 -and $element.to.Count -eq 3) "Invalid cuboid in $relative"
            for ($axis=0; $axis -lt 3; $axis++) { Assert-Asset ($element.to[$axis] -ge $element.from[$axis]) "Inverted cuboid in $relative" }
            foreach ($face in $element.faces.Values) {
                $textureName = ([string]$face.texture).TrimStart('#')
                Assert-Asset ($data.textures.Contains($textureName)) "Missing texture slot $textureName in $relative"
                Assert-Asset ($face.uv.Count -eq 4) "Invalid face UV in $relative"
            }
            foreach ($child in $element.children) { Check-Element $child }
        }
        foreach ($element in $data.elements) { Check-Element $element }
        foreach ($animation in $data.animations) {
            foreach ($frame in $animation.keyframes) {
                foreach ($name in $frame.elements.Keys) { Assert-Asset ($names.Contains($name)) "Unknown animation target $name in $relative" }
            }
        }
        foreach ($texture in $data.textures.Values) { Check-Reference $texture 'textures' '.png' }
    }
    if ($relative.StartsWith('entities/')) {
        Check-Reference $data.client.shape.base 'shapes' '.json'
        foreach ($texture in $data.client.textures.Values) { Check-Reference $texture.base 'textures' '.png' }
        if ($data.code -like 'chanter-*') {
            Assert-Asset ($data.server.behaviors.Where({$_.code -eq 'taskai'}).Count -eq 0) 'Chanters must remain passive.'
            $health = $data.server.behaviors.Where({$_.code -eq 'health'})
            Assert-Asset ($health.Count -eq 1 -and $health[0].maxhealth -eq 40) 'Chanter health differs from design.'
        }
        if ($data.code -eq 'openingbell') {
            Assert-Asset ($data.class -eq 'EntityAgent') 'Opening bell must not independently spawn children.'
            Assert-Asset ($data.server.behaviors.Where({$_.code -eq 'taskai'}).Count -eq 0) 'Opening bell must use shared coordinator.'
        }
    }
    if ($relative.StartsWith('blocktypes/') -or $relative.StartsWith('itemtypes/')) {
        if ($data.shape.base) { Check-Reference $data.shape.base 'shapes' '.json' }
        foreach ($shape in $data.shapebytype.Values) { Check-Reference $shape.base 'shapes' '.json' }
        foreach ($texture in $data.textures.Values) { Check-Reference $texture.base 'textures' '.png' }
    }
}
$lang = Get-Content -LiteralPath (Join-Path $assetRoot 'lang/en.json') -Raw | ConvertFrom-Json -AsHashtable
foreach ($folder in @('entities','itemtypes','blocktypes')) {
    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $assetRoot $folder) -File -Filter '*.json') {
        $data = Get-Content -LiteralPath $file.FullName -Raw | ConvertFrom-Json -AsHashtable
        $prefix = @{ entities='entity'; itemtypes='item'; blocktypes='block' }[$folder]
        Assert-Asset ($lang.Contains("$prefix-$($data.code)") -or $lang.Contains("$prefix-$($data.code)-*")) "Missing name $prefix-$($data.code)"
    }
}
Write-Output "$($jsonFiles.Count) JSON files; $script:checks asset checks passed. This is static validation, not in-game rendering."

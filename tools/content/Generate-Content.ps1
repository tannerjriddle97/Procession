$ErrorActionPreference = 'Stop'
$root = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$assetRoot = Join-Path $root 'assets/procession'
function Write-Json($relative, $value) {
    $path = Join-Path $assetRoot $relative
    [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($path)) | Out-Null
    [System.IO.File]::WriteAllText($path, ($value | ConvertTo-Json -Depth 100), [System.Text.UTF8Encoding]::new($false))
}

# Original, deterministic 16px pixel materials. No game texture is copied.
Add-Type -AssemblyName System.Drawing
$palettes = [ordered]@{
    charcoal = @(35,40,38); rust = @(103,65,41); linen = @(151,140,108)
    timber = @(74,62,45); teal = @(51,154,139); dark = @(15,22,22); stone = @(94,99,91)
}
foreach ($entry in $palettes.GetEnumerator()) {
    $bmp = [System.Drawing.Bitmap]::new(16,16)
    for ($y=0; $y -lt 16; $y++) { for ($x=0; $x -lt 16; $x++) {
        $noise = (($x*31 + $y*17 + $x*$y*7) % 19)-9
        if ($entry.Key -eq 'linen' -and ($y%4 -eq 0)) { $noise -= 12 }
        if ($entry.Key -eq 'timber' -and ($x%5 -eq 0)) { $noise -= 11 }
        $rgb = @($entry.Value | ForEach-Object { [Math]::Clamp($_+$noise,0,255) })
        $bmp.SetPixel($x,$y,[System.Drawing.Color]::FromArgb(255,$rgb[0],$rgb[1],$rgb[2]))
    }}
    $texturePath = Join-Path $assetRoot "textures/material/$($entry.Key).png"
    [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($texturePath)) | Out-Null
    $bmp.Save($texturePath,[System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
}
$script:textures = [ordered]@{}
foreach ($material in $palettes.Keys) { $script:textures[$material] = "procession:material/$material" }
$script:meshes = @{}
function Cube($name, $from, $to, $material, $pivot = $null) {
    $faces = [ordered]@{}
    foreach ($side in @('north','east','south','west','up','down')) {
        $face = [ordered]@{texture="#$material"; uv=@(0,0,8,8)}
        if ($material -eq 'teal') { $face.glow = 90 }
        $faces[$side] = $face
    }
    $element = [ordered]@{name=$name; from=$from; to=$to; faces=$faces}
    if ($null -ne $pivot) { $element.rotationOrigin = $pivot }
    return $element
}
function Animation($name, $frames, $keyframes, $end = 'Repeat') {
    return [ordered]@{name=$name; code=$name; quantityframes=$frames; onActivityStopped='EaseOut'; onAnimationEnd=$end; keyframes=$keyframes}
}
function Write-Shape($name, $elements, $animated=$false) {
    $shape = [ordered]@{editor=@{allAngles=$true; entityTextureMode=$animated}; textureWidth=16; textureHeight=16; textures=$script:textures; elements=@([ordered]@{name='origin'; from=@(0,0,0); to=@(0,0,0); rotationOrigin=@(8,0,8); faces=@{}; children=$elements})}
    if ($animated) {
        $shape.animations = @(
            (Animation 'idle' 80 @(@{frame=0;elements=@{origin=@{offsetY=0}}},@{frame=40;elements=@{origin=@{offsetY=0.28}}},@{frame=79;elements=@{origin=@{offsetY=0}}})),
            (Animation 'walk' 24 @(@{frame=0;elements=@{legleft=@{rotationX=18};legright=@{rotationX=-18}}},@{frame=12;elements=@{legleft=@{rotationX=-18};legright=@{rotationX=18}}},@{frame=23;elements=@{legleft=@{rotationX=18};legright=@{rotationX=-18}}})),
            (Animation 'attack' 36 @(@{frame=0;elements=@{armleft=@{rotationX=0};armright=@{rotationX=0}}},@{frame=12;elements=@{armleft=@{rotationX=-70};armright=@{rotationX=-25}}},@{frame=19;elements=@{armleft=@{rotationX=25};armright=@{rotationX=10}}},@{frame=35;elements=@{armleft=@{rotationX=0};armright=@{rotationX=0}}}) 'Stop'),
            (Animation 'hurt' 10 @(@{frame=0;elements=@{origin=@{rotationZ=0}}},@{frame=3;elements=@{origin=@{rotationZ=4}}},@{frame=9;elements=@{origin=@{rotationZ=0}}}) 'Stop'),
            (Animation 'die' 30 @(@{frame=0;elements=@{origin=@{rotationX=0}}},@{frame=29;elements=@{origin=@{rotationX=85;offsetY=2}}}) 'Hold')
        )
    }
    Write-Json "shapes/$name.json" $shape
    $script:meshes[$name] = $elements
}
function Humanoid-Base($material='charcoal') {
    return @(
        (Cube 'legleft' @(4,0,6) @(7,11,10) $material @(5.5,11,8)),
        (Cube 'legright' @(9,0,6) @(12,11,10) $material @(10.5,11,8)),
        (Cube 'body' @(3,11,5) @(13,23,11) $material),
        (Cube 'armleft' @(-1,10,5) @(3,23,10) $material @(1,22,8)),
        (Cube 'armright' @(13,10,5) @(17,23,10) $material @(15,22,8))
    )
}
$veiled = @(Humanoid-Base 'charcoal') + @(
    (Cube 'hood' @(3.5,23,4.5) @(12.5,33,12) 'charcoal'),
    (Cube 'facerecess' @(5,25,4.25) @(11,30,4.5) 'dark'),
    (Cube 'mantle' @(1.5,19,4) @(14.5,24,12) 'linen'),
    (Cube 'mouthglow' @(5.5,25.5,4) @(10.5,26.5,4.25) 'teal'),
    (Cube 'grilleleft' @(6,25,3.75) @(6.5,28,4) 'rust'),
    (Cube 'grillemiddle' @(7.75,25,3.75) @(8.25,28,4) 'rust'),
    (Cube 'grilleright' @(9.5,25,3.75) @(10,28,4) 'rust'),
    (Cube 'tabard' @(5,10,4.5) @(11,20,5) 'linen')
)
Write-Shape 'entity/chanter-veiled' $veiled $true
$bound = @(
    (Cube 'legleft' @(3,0,6) @(7,9,11) 'charcoal' @(5,9,8)),
    (Cube 'legright' @(9,0,6) @(13,9,11) 'charcoal' @(11,9,8)),
    (Cube 'body' @(2,9,5) @(14,20,13) 'linen'),
    (Cube 'armleft' @(-2,2,3) @(2,20,8) 'linen' @(0,19,6)),
    (Cube 'armright' @(14,2,3) @(18,20,8) 'linen' @(16,19,6)),
    (Cube 'head' @(4,19,1) @(12,26,8) 'linen'),
    (Cube 'faceband' @(3.5,22,0.5) @(12.5,24,1) 'charcoal'),
    (Cube 'eyes' @(5,22.4,0.25) @(11,23,0.5) 'teal'),
    (Cube 'binding1' @(1.5,12,4.5) @(14.5,14,13.5) 'charcoal'),
    (Cube 'binding2' @(1.5,17,4.5) @(14.5,19,13.5) 'charcoal')
)
Write-Shape 'entity/chanter-bound' $bound $true
$resonant = @(Humanoid-Base 'rust') + @(
    (Cube 'chestplate' @(2,13,4) @(14,22,5) 'rust'),
    (Cube 'chestseam' @(7.5,14,3.75) @(8.5,21,4) 'teal'),
    (Cube 'cagebase' @(3.5,23,4) @(12.5,25,12) 'rust'),
    (Cube 'cagetop' @(3.5,31,4) @(12.5,33,12) 'rust'),
    (Cube 'darkinterior' @(5,25,6) @(11,31,11) 'dark'),
    (Cube 'cage1' @(3.5,25,4) @(5,31,5.5) 'rust'),
    (Cube 'cage2' @(7.25,25,4) @(8.75,31,5.5) 'rust'),
    (Cube 'cage3' @(11,25,4) @(12.5,31,5.5) 'rust'),
    (Cube 'hangingplate' @(4,7,4.5) @(12,12,5) 'linen')
)
Write-Shape 'entity/chanter-resonant' $resonant $true
$hollow = @(Humanoid-Base 'charcoal') + @(
    (Cube 'shoulderbrace' @(0.5,21,4) @(15.5,24,12) 'timber'),
    (Cube 'forkbase' @(3,24,5) @(13,27,11) 'rust'),
    (Cube 'forkleft' @(3,27,5) @(6,37,11) 'rust'),
    (Cube 'forkright' @(10,27,5) @(13,37,11) 'rust'),
    (Cube 'innerleft' @(5.75,28,5.25) @(6,36.5,10.75) 'teal'),
    (Cube 'innerright' @(10,28,5.25) @(10.25,36.5,10.75) 'teal'),
    (Cube 'bracedchest' @(4,13,4) @(12,19,5) 'linen')
)
Write-Shape 'entity/chanter-hollow' $hollow $true
$custodian = @(
    (Cube 'legleft' @(-4,0,3) @(4,14,14) 'rust' @(0,14,8)),
    (Cube 'legright' @(12,0,3) @(20,14,14) 'rust' @(16,14,8)),
    (Cube 'leftbrace' @(-7,0,0) @(5,3,17) 'timber'),
    (Cube 'rightbrace' @(11,0,0) @(23,3,17) 'timber'),
    (Cube 'body' @(-1,14,2) @(17,34,15) 'timber'),
    (Cube 'chassisleft' @(-3,12,0) @(1,39,16) 'rust'),
    (Cube 'chassisright' @(15,12,0) @(19,39,16) 'rust'),
    (Cube 'housing' @(-3,34,0) @(19,40,16) 'charcoal'),
    (Cube 'interior' @(1,18,1) @(15,32,2) 'dark'),
    (Cube 'slat1' @(2,18,0) @(4,33,1) 'rust'),
    (Cube 'slat2' @(7,18,0) @(9,33,1) 'rust'),
    (Cube 'slat3' @(12,18,0) @(14,33,1) 'rust'),
    (Cube 'core' @(5,22,0.75) @(11,28,1) 'teal'),
    (Cube 'armleft' @(-13,10,1) @(-4,31,12) 'rust' @(-5,29,8)),
    (Cube 'supportplate' @(-16,8,-2) @(-3,21,2) 'charcoal'),
    (Cube 'armright' @(20,9,5) @(25,31,10) 'timber' @(21,29,8)),
    (Cube 'recoverycleat' @(20,4,0) @(25,10,10) 'rust'),
    (Cube 'repairstrap' @(-2,27,0) @(18,30,16) 'linen')
)
Write-Shape 'entity/lastcustodian' $custodian $true
$bell = @(
    (Cube 'base' @(2,0,2) @(14,2,14) 'timber'),
    (Cube 'postleft' @(2,2,5) @(4,14,11) 'rust'),
    (Cube 'postright' @(12,2,5) @(14,14,11) 'rust'),
    (Cube 'crossbar' @(2,13,5) @(14,16,11) 'timber'),
    (Cube 'bellbody' @(5,6,5) @(11,12,11) 'rust'),
    (Cube 'bellrim' @(4.5,5,4.5) @(11.5,7,11.5) 'charcoal'),
    (Cube 'clapper' @(7,3,7) @(9,6,9) 'teal'),
    (Cube 'lever' @(10,2,2) @(12,9,4) 'rust'),
    (Cube 'leverhandle' @(9.5,8,1.5) @(12.5,10,4.5) 'linen')
)
Write-Shape 'block/closingbell' $bell
Write-Shape 'item/relayfragment' @((Cube 'fragment' @(3,6,5) @(13,9,11) 'rust'),(Cube 'seam' @(5,9,6) @(11,9.5,10) 'teal'))
Write-Shape 'item/routechart' @((Cube 'baton' @(7,1,7) @(9,14,9) 'timber'),(Cube 'plate' @(3,8,5) @(13,12,7) 'rust'),(Cube 'mark' @(7,8.5,4.75) @(8,11.5,5) 'teal'))
Write-Shape 'block/guidepost' @(
    (Cube 'post' @(6,0,6) @(10,16,10) 'timber'),
    (Cube 'board' @(2,10,2) @(14,14,5) 'linen'),
    (Cube 'arrowshaft' @(4,11.5,1.75) @(11,12.5,2) 'charcoal'),
    (Cube 'arrowhead' @(10,10.5,1.5) @(12,13.5,1.75) 'rust'),
    (Cube 'binding' @(5.5,7,5.5) @(10.5,9,10.5) 'linen')
)
Write-Shape 'block/memorialboard' @(
    (Cube 'base' @(1,0,3) @(15,2,13) 'stone'),
    (Cube 'postleft' @(2,2,6) @(4,15,10) 'timber'),
    (Cube 'postright' @(12,2,6) @(14,15,10) 'timber'),
    (Cube 'board' @(1,7,5) @(15,15,7) 'timber'),
    (Cube 'plate1' @(2,8,4.5) @(4,13,5) 'rust'),
    (Cube 'plate2' @(5.5,8,4.5) @(7.5,13,5) 'rust'),
    (Cube 'plate3' @(9,8,4.5) @(11,13,5) 'rust'),
    (Cube 'plate4' @(12,8,4.5) @(14,13,5) 'rust'),
    (Cube 'binding' @(1,14,4.75) @(15,15,7.25) 'linen')
)
Write-Shape 'block/relaylamp' @(
    (Cube 'foot' @(3,0,3) @(13,2,13) 'charcoal'),
    (Cube 'glass' @(5,3,5) @(11,11,11) 'teal'),
    (Cube 'post1' @(4,2,4) @(6,12,6) 'rust'),
    (Cube 'post2' @(10,2,4) @(12,12,6) 'rust'),
    (Cube 'post3' @(4,2,10) @(6,12,12) 'rust'),
    (Cube 'post4' @(10,2,10) @(12,12,12) 'rust'),
    (Cube 'roof' @(3,12,3) @(13,14,13) 'charcoal'),
    (Cube 'handleleft' @(6,14,7) @(7,16,9) 'rust'),
    (Cube 'handleright' @(9,14,7) @(10,16,9) 'rust')
)

function Entity-Definition($code,$hp,$behavior,$height) {
    $textureDefs = [ordered]@{}
    foreach ($material in $palettes.Keys) { $textureDefs[$material] = @{base="procession:material/$material"} }
    $server = @(@{code='repulseagents'},@{code='controlledphysics';stepHeight=0.6})
    if ($behavior) { $server += @{code=$behavior} }
    $server += @(@{code='health';currenthealth=$hp;maxhealth=$hp},@{code='deaddecay';hoursToDecay=0.1})
    $definition = [ordered]@{
        code=$code; class='EntityAgent'; tags=@('mechanical','habitat-land'); weight=200; canClimb=$false
        hitboxSize=@{x=0.8;y=$height}; selectionBoxSize=@{x=0.9;y=$height}; deadHitboxSize=@{x=0.8;y=0.35}; eyeHeight=($height-0.15)
        attributes=@{glitchAffected=$true;isMechanical=$true;onDeathStabilityRecovery=0}; drops=@()
        client=@{renderer='Shape';shape=@{base="procession:entity/$code"};textures=$textureDefs;size=1;behaviors=@(@{code='repulseagents'},@{code='controlledphysics';stepHeight=0.6},@{code='interpolateposition'});animations=@(
            @{code='idle';animation='idle';blendMode='Average';triggeredBy=@{defaultAnim=$true}},
            @{code='walk';animation='walk';blendMode='Average'},@{code='run';animation='walk';blendMode='Average'},
            @{code='attack';animation='attack';blendMode='Average';weight=10},@{code='hurt';animation='hurt';blendMode='AddAverage';weight=10},
            @{code='die';animation='die';blendMode='Average';weight=20;triggeredBy=@{onControls=@('dead')}}
        )}; server=@{behaviors=$server}
    }
    return $definition
}
foreach ($variant in @('bound','veiled','resonant','hollow')) {
    $height = if ($variant -eq 'hollow') {2.35} elseif ($variant -eq 'bound') {1.65} else {2.1}
    $definition = Entity-Definition "chanter-$variant" 40 'ProcessionChanter' $height
    $definition.drops = @(@{type='item';code='procession:relayfragment';quantity=@{avg=1;var=0}})
    Write-Json "entities/chanter-$variant.json" $definition
}
$boss = Entity-Definition 'lastcustodian' 420 'ProcessionBoss' 2.5
$boss.hitboxSize.x=1.8; $boss.selectionBoxSize.x=2.0; $boss.weight=1000
$boss.server.behaviors += @{code='taskai';aitasks=@(
    @{code='meleeattack';entityCodes=@('game:player');priority=2;damage=9;damageTier=2;damageType='BluntAttack';mincooldown=1400;maxcooldown=2100;damagePlayerAtMs=650;attackDurationMs=1200;animation='attack';animationSpeed=1;slot=1},
    @{code='seekentity';entityCodes=@('game:player');priority=1.5;mincooldown=300;maxcooldown=500;searchWaitMs=1200;movespeed=0.015;executionChance=1;seekingRange=24;animation='walk';animationSpeed=0.8},
    @{code='idle';priority=0.5;minduration=1500;maxduration=2500;animation='idle'}
)}
Write-Json 'entities/lastcustodian.json' $boss
$opening = Entity-Definition 'openingbell' 100 '' 1.5
$opening.client.shape = @{base='game:entity/lore/bell/bell'}
$opening.client.textures = @{bell=@{base='game:entity/lore/bell/bell'}}
$opening.client.size=1.3
# The custom coordinator, never the stock bellalarm AI, owns every reinforcement.
$opening.client.animations = @(@{code='idle';animation='idle';blendMode='Average';triggeredBy=@{defaultAnim=$true}},@{code='die';animation='die';blendMode='Average';weight=20;triggeredBy=@{onControls=@('dead')}})
Write-Json 'entities/openingbell.json' $opening

$blockTextures=[ordered]@{}
foreach ($material in $palettes.Keys) { $blockTextures[$material]=@{base="procession:material/$material"} }
Write-Json 'blocktypes/closingbell.json' ([ordered]@{
    code='closingbell'; class='ProcessionClosingBell'; shape=@{base='procession:block/closingbell'}; textures=$blockTextures
    creativeinventory=@{general=@('*');decorative=@('*')}; blockmaterial='Metal'; drawtype='json'; replaceable=0; resistance=4
    lightAbsorption=0; lightHsv=@(25,3,4); sidesolid=@{all=$false}; sideopaque=@{all=$false}
    collisionbox=@{x1=0.125;y1=0;z1=0.125;x2=0.875;y2=1;z2=0.875}; selectionbox=@{x1=0.125;y1=0;z1=0.125;x2=0.875;y2=1;z2=0.875}
    sounds=@{place='game:block/heavymetal-hit';hit='game:block/heavymetal-hit';break='game:block/heavymetal-hit'}; maxstacksize=1
    guiTransform=@{rotation=@{x=-15;y=25;z=0};scale=1.1}; attributes=@{handbook=@{extraSections=@(@{title='A choice earned';text='Defeat the Last Custodian at the Far Relay. Place this instrument and interact to choose whether future heavy storms contain a Procession. Ordinary temporal storms continue.'})}}
})
Write-Json 'itemtypes/relayfragment.json' ([ordered]@{code='relayfragment';shape=@{base='procession:item/relayfragment'};textures=$blockTextures;maxstacksize=64;creativeinventory=@{general=@('*');items=@('*')};guiTransform=@{rotation=@{x=-25;y=20;z=15};scale=2.5};attributes=@{handbook=@{extraSections=@(@{title='Recovered signal';text='A guaranteed remnant of a storm chanter. Combine it with a stick to make a route chart. It is a guide, not a key.'})}}})
Write-Json 'itemtypes/routechart.json' ([ordered]@{code='routechart';class='ProcessionRouteChart';shape=@{base='procession:item/routechart'};textures=$blockTextures;maxstacksize=1;creativeinventory=@{general=@('*');items=@('*')};guiTransform=@{rotation=@{x=-15;y=20;z=15};scale=1.8};attributes=@{handbook=@{extraSections=@(@{title='Follow the return';text='Interact while holding this chart to read the direction to the next relay. It is reusable. The Far Relay can also be found and entered directly.'})}}})
Write-Json 'recipes/grid/routechart.json' @(
    ([ordered]@{ingredientPattern='FS';width=2;height=1;ingredients=@{F=@{type='item';code='procession:relayfragment'};S=@{type='item';code='game:stick'}};output=@{type='item';code='procession:routechart';quantity=1}}),
    ([ordered]@{ingredientPattern='LCS';width=3;height=1;ingredients=@{L=@{type='item';code='game:cloth-plain'};C=@{type='item';code='game:charcoal'};S=@{type='item';code='game:stick'}};output=@{type='item';code='procession:routechart';quantity=1}})
)
foreach ($furnishing in @('guidepost','memorialboard','relaylamp')) {
    $furnishingBlock=[ordered]@{
        code=$furnishing; shape=@{base="procession:block/$furnishing"}; textures=$blockTextures
        creativeinventory=@{general=@('*');decorative=@('*')}; blockmaterial='Wood'; drawtype='json'; replaceable=0; resistance=2
        lightAbsorption=0; sidesolid=@{all=$false}; sideopaque=@{all=$false}
        collisionbox=@{x1=0.125;y1=0;z1=0.125;x2=0.875;y2=1;z2=0.875};selectionbox=@{x1=0.125;y1=0;z1=0.125;x2=0.875;y2=1;z2=0.875}
        sounds=@{place='game:block/planks';hit='game:block/planks';break='game:block/planks'}
        guiTransform=@{rotation=@{x=-15;y=25;z=0};scale=1.1}
    }
    if ($furnishing -in @('guidepost','memorialboard')) {
        $furnishingBlock.variantgroups=@(@{code='side';states=@('north','east','south','west')})
        $furnishingBlock.behaviors=@(@{name='HorizontalOrientable'})
        $furnishingBlock.Remove('shape')
        $furnishingBlock.shapebytype=[ordered]@{}
        $turn=0
        foreach($direction in @('north','east','south','west')) {
            $furnishingBlock.shapebytype["*-$direction"]=@{base="procession:block/$furnishing";rotateY=$turn}
            $turn+=270; $turn=$turn%360
        }
        $furnishingBlock.creativeinventory=@{general=@("$furnishing-north");decorative=@("$furnishing-north")}
    }
    if ($furnishing -eq 'relaylamp') { $furnishingBlock.blockmaterial='Metal';$furnishingBlock.lightHsv=@(25,3,11) }
    Write-Json "blocktypes/$furnishing.json" $furnishingBlock
}
Write-Json 'recipes/grid/guidepost.json' ([ordered]@{ingredientPattern="SSS`t_T_`t_S_";width=3;height=3;ingredients=@{S=@{type='item';code='game:stick'};T=@{type='item';code='game:flaxtwine'}};output=@{type='block';code='procession:guidepost-north';quantity=1}})
Write-Json 'recipes/grid/memorialboard.json' ([ordered]@{ingredientPattern="MSM`tSSS`tS_S";width=3;height=3;ingredients=@{S=@{type='item';code='game:stick'};M=@{type='block';code='game:metal-scraps'}};output=@{type='block';code='procession:memorialboard-north';quantity=1}})
Write-Json 'recipes/grid/relaylamp.json' ([ordered]@{ingredientPattern="_S_`tTFT`t_S_";width=3;height=3;ingredients=@{S=@{type='item';code='game:stick'};T=@{type='item';code='game:flaxtwine'};F=@{type='item';code='procession:relayfragment'}};output=@{type='block';code='procession:relaylamp';quantity=1}})
Write-Json 'lang/en.json' ([ordered]@{
    'item-relayfragment'='Relay Fragment';'item-routechart'='Route Chart';'block-closingbell'='Closing Bell'
    'entity-chanter-bound'='The Bound';'entity-chanter-veiled'='The Veiled';'entity-chanter-resonant'='The Resonant';'entity-chanter-hollow'='The Hollow'
    'item-creature-chanter-bound'='The Bound';'item-creature-chanter-veiled'='The Veiled';'item-creature-chanter-resonant'='The Resonant';'item-creature-chanter-hollow'='The Hollow'
    'entity-lastcustodian'='The Last Custodian';'entity-openingbell'='Procession Bell'
    'item-creature-lastcustodian'='The Last Custodian';'item-creature-openingbell'='Procession Bell'
    'control-allowed'='Procession allowed world-wide. Applies at the next heavy storm.'
    'control-suppressed'='Procession suppressed world-wide. Ordinary temporal storms continue. Applies at the next heavy storm.'
    'block-guidepost'='Field Guide-Post';'block-guidepost-*'='Field Guide-Post';'block-memorialboard'='Four-Plate Memorial';'block-memorialboard-*'='Four-Plate Memorial';'block-relaylamp'='Relay Lamp'
})
Write-Host 'Generated original meshes, deterministic textures, entity/block/item definitions and route-chart recipe.'

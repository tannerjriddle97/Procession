$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$projectRoot=[System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$bitmap=[System.Drawing.Bitmap]::new(1400,1090)
$graphics=[System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::FromArgb(26,32,33))
$graphics.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::None
$font=[System.Drawing.Font]::new('Segoe UI',17,[System.Drawing.FontStyle]::Bold)
$small=[System.Drawing.Font]::new('Segoe UI',10)
$white=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(220,224,212))
$colors=@{charcoal=@(35,40,38);rust=@(103,65,41);linen=@(151,140,108);timber=@(74,62,45);teal=@(51,154,139);dark=@(15,22,22);stone=@(94,99,91)}
$models=@(
    @{file='entity/chanter-veiled';label='The Veiled';x=150;y=345;scale=7},
    @{file='entity/chanter-bound';label='The Bound';x=415;y=345;scale=7},
    @{file='entity/chanter-resonant';label='The Resonant';x=680;y=345;scale=7},
    @{file='entity/chanter-hollow';label='The Hollow';x=965;y=345;scale=7},
    @{file='entity/lastcustodian';label='The Last Custodian';x=350;y=705;scale=6.1},
    @{file='block/closingbell';label='Closing Bell';x=760;y=705;scale=10},
    @{file='block/guidepost';label='Field Guide-Post';x=150;y=1015;scale=8},
    @{file='block/memorialboard';label='Four-Plate Memorial';x=420;y=1015;scale=8},
    @{file='block/relaylamp';label='Relay Lamp';x=690;y=1015;scale=8},
    @{file='item/relayfragment';label='Relay Fragment';x=960;y=1015;scale=8},
    @{file='item/routechart';label='Route Chart';x=1225;y=1015;scale=8}
)
function Project($x,$y,$z,$m) { return [System.Drawing.PointF]::new([single]($m.x+($x-$z)*$m.scale*.72),[single]($m.y+($x+$z-16)*$m.scale*.24-$y*$m.scale)) }
foreach($model in $models) {
    $shape=Get-Content (Join-Path $projectRoot "assets/procession/shapes/$($model.file).json") -Raw | ConvertFrom-Json
    $parts=$shape.elements[0].children | Sort-Object { -($_.from[0]+$_.to[0]+$_.from[2]+$_.to[2]) }
    foreach($part in $parts) {
        $x1=$part.from[0];$y1=$part.from[1];$z1=$part.from[2];$x2=$part.to[0];$y2=$part.to[1];$z2=$part.to[2]
        $material=$part.faces.north.texture.Substring(1);$rgb=$colors[$material]
        $faces=@(
            @{brightness=.72;points=@((Project $x1 $y1 $z1 $model),(Project $x1 $y1 $z2 $model),(Project $x1 $y2 $z2 $model),(Project $x1 $y2 $z1 $model))},
            @{brightness=.95;points=@((Project $x1 $y1 $z1 $model),(Project $x2 $y1 $z1 $model),(Project $x2 $y2 $z1 $model),(Project $x1 $y2 $z1 $model))},
            @{brightness=1.2;points=@((Project $x1 $y2 $z1 $model),(Project $x2 $y2 $z1 $model),(Project $x2 $y2 $z2 $model),(Project $x1 $y2 $z2 $model))}
        )
        foreach($face in $faces) {
            $shade=@($rgb | ForEach-Object {[int][Math]::Min(255,$_*$face.brightness)})
            $brush=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb($shade[0],$shade[1],$shade[2]))
            $graphics.FillPolygon($brush,[System.Drawing.PointF[]]$face.points)
            $brush.Dispose()
        }
    }
    $graphics.DrawString($model.label,$font,$white,([single]($model.x-80)),([single]($model.y+20)))
}
$graphics.DrawString('Original cuboid mesh preview - geometry check, not an in-game screenshot',$small,$white,35,14)
$graphics.DrawString('Hollow uses an open U-shaped head. 7 original 16px materials; simple idle / walk / attack / hurt / death animation sets.',$small,$white,35,38)
$preview=Join-Path $PSScriptRoot 'mesh-preview.png'
$bitmap.Save($preview,[System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose();$bitmap.Dispose();$font.Dispose();$small.Dispose();$white.Dispose()
Write-Host $preview

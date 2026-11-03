# Create minimal valid PNG files (1x1 pixel transparent PNG)
# This is a base64 encoded minimal PNG file

$pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="
$pngBytes = [Convert]::FromBase64String($pngBase64)

# List of required asset files
$assets = @(
    "LockScreenLogo.scale-200.png",
    "SplashScreen.scale-200.png",
    "Square150x150Logo.scale-200.png",
    "Square44x44Logo.scale-200.png",
    "Square44x44Logo.targetsize-24_altform-unplated.png",
    "Wide310x150Logo.scale-200.png",
    "StoreLogo.scale-200.png"
)

$assetPath = "C:\AppCodeStore\AI-Model-Code\TestSigningApp\Assets"

foreach ($asset in $assets) {
    $filePath = Join-Path $assetPath $asset
    [System.IO.File]::WriteAllBytes($filePath, $pngBytes)
    Write-Host "Created: $filePath"
}

Write-Host "All asset files created successfully!"

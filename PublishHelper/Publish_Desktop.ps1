Set-Location ".."

$path = ".\PublishHelper\QrGenerator"

dotnet publish "QrGenerator.Desktop" `
    -f "net8.0-windows10.0.26100.0" `
    -c "Release" `
    -p:RuntimeIdentifierOverride=win10-x64 `
    -p:WindowsPackageType=None `
    --verbosity quiet `
    --self-contained true `
    -o $path

Remove-Item "$($path)\*.pdb"

Compress-Archive -Path $path -DestinationPath "$($path).zip"

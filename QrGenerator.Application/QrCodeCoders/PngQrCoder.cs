using QRCoder;
using QrGenerator.Application.Extensions;
using System.Drawing;

namespace QrGenerator.Application.QrCodeCoders;

public class PngQrCoder
{
    private const int QrCodeSize = 500;

    public void CreateWiFiQrFile(
        string ssid,
        string password,
        string filePath,
        string? authType = null,
        Bitmap? bitmap = null)
    {
        var bitmapQr = GetWiFiQr(ssid, password, authType, bitmap);
        bitmapQr.Save(filePath);
    }

    public Bitmap GetWiFiQr(
        string ssid,
        string password,
        string? authType = null,
        Bitmap? bitmap = null)
    {
        var type = authType is null ? PayloadGenerator.WiFi.Authentication.WPA :
            Enum.Parse<PayloadGenerator.WiFi.Authentication>(authType);
        Bitmap bitmapResult;
        var wifiPayload = new PayloadGenerator.WiFi(ssid, password, type);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new QRCode(qrCodeData))
        {
            bitmapResult = qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, icon: bitmap?.AddCaption(ssid));
        }

        return bitmapResult;
    }
}

using QRCoder;
using QrGenerator.Application.Extensions;
using System.Drawing;

namespace QrGenerator.Application.QrCodeCoders;

public static class PngQrCoder
{
    private const int QrCodeSize = 500;

    public static Bitmap GetWiFiQr(
        string? ssid,
        string? password,
        string? authType = null,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(ssid);
        ArgumentNullException.ThrowIfNull(password);

        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);
        var type = authType is null ?
            PayloadGenerator.WiFi.Authentication.WPA :
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

    public static Bitmap GetQr(
        string? content,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);

        Bitmap bitmapResult;
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new QRCode(qrCodeData))
        {
            bitmapResult = qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, icon: bitmap);
        }

        return bitmapResult;
    }

    public static Bitmap GetUrlQr(
        string? content,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);
        var urlPayload = new PayloadGenerator.Url(content);

        Bitmap bitmapResult;
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(urlPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new QRCode(qrCodeData))
        {
            bitmapResult = qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, icon: bitmap?.AddCaption(content.GetDomain()));
        }

        return bitmapResult;
    }
}

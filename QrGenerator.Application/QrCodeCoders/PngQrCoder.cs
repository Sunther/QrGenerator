using System.Drawing;
using QRCoder;
using QrGenerator.Application.Extensions;

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

        var type = authType is null ?
            PayloadGenerator.WiFi.Authentication.WPA :
            Enum.Parse<PayloadGenerator.WiFi.Authentication>(authType);

        var wifiPayload = new PayloadGenerator.WiFi(ssid, password, type);

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        using var memory = new MemoryStream(qrCode.GetGraphic(QrCodeSize));

        return new Bitmap(memory);
    }

    public static Bitmap GetQr(
        string? content,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        using var bitmap = TryLoadBitmap(imagePath);

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

        var urlPayload = new PayloadGenerator.Url(content);

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        using var memory = new MemoryStream(qrCode.GetGraphic(QrCodeSize));

        return new Bitmap(memory);
    }

    private static Bitmap? TryLoadBitmap(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return null;
        }

        try
        {
            // This can throw if the file is invalid; we catch and return null.
            return new Bitmap(imagePath);
        }
        catch
        {
            // TODO: optionally log the problem or propagate a custom error
            return null;
        }
    }
}

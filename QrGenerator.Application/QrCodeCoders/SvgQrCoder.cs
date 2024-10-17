using System.Drawing;
using QRCoder;
using QrGenerator.Application.Extensions;
using QrGenerator.Disk;
using static QRCoder.SvgQRCode;

namespace QrGenerator.Application.QrCodeCoders;

public class SvgQrCoder
{
    private const int QrCodeSize = 500;
    private readonly FileWriter _fileWriter;

    public SvgQrCoder()
    {
        _fileWriter = new FileWriter();
    }

    public void CreateBasicFile(
        string? content,
        string filePath,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, true, SizingMode.WidthHeightAttribute, ConvertToSvgLog(bitmap)));
        }
    }

    public void CreateWiFiFile(
        string? ssid,
        string? password,
        string filePath,
        string? authType = null,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(ssid);
        ArgumentNullException.ThrowIfNull(password);

        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);
        var type = authType is null ?
            PayloadGenerator.WiFi.Authentication.WPA :
            Enum.Parse<PayloadGenerator.WiFi.Authentication>(authType);

        var wifiPayload = new PayloadGenerator.WiFi(ssid, password, type);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, true, SizingMode.WidthHeightAttribute,
                    ConvertToSvgLog(bitmap, ssid)));

        }
    }

    public void CreateUrlFile(
        string? content,
        string filePath,
        string? imagePath = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        var bitmap = string.IsNullOrEmpty(imagePath) ? null : new Bitmap(imagePath);
        var urlPayload = new PayloadGenerator.Url(content);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(urlPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(QrCodeSize, Color.Black, Color.White, drawQuietZones: true, SizingMode.WidthHeightAttribute,
                    ConvertToSvgLog(bitmap, content.GetDomain())));
        }
    }

    private static SvgLogo? ConvertToSvgLog(Bitmap? bitmap, string? ssid = "")
    {
        if (bitmap is null)
        {
            return null;
        }

        var img = bitmap.AddCaption(ssid);

        return new SvgLogo(img, fillLogoBackground: false);
    }
}

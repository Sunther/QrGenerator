using System.Drawing;
using QRCoder;
using QrGenerator.Application.Extensions;
using QrGenerator.Disk;
using static QRCoder.SvgQRCode;

namespace QrGenerator.Application.QrCodeCoders;

public class SvgQrCoder
{
    private readonly FileWriter _fileWriter;

    public SvgQrCoder()
    {
        _fileWriter = new FileWriter();
    }

    public void CreateBasicFile(
        string content,
        string filePath,
        Bitmap? bitmap = null)
    {
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(20, Color.Black, Color.White, true, SizingMode.WidthHeightAttribute, ConvertToSvgLog(bitmap)));
        }
    }

    public void CreateWiFiFile(
        string ssid,
        string password,
        string filePath,
        PayloadGenerator.WiFi.Authentication type = PayloadGenerator.WiFi.Authentication.WPA,
        Bitmap? bitmap = null)
    {
        var wifiPayload = new PayloadGenerator.WiFi(ssid, password, PayloadGenerator.WiFi.Authentication.WPA);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(20, Color.Black, Color.White, true, SizingMode.WidthHeightAttribute, ConvertToSvgLog(bitmap, ssid)));

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

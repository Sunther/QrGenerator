using System.Drawing;
using QRCoder;
using QrGenerator.Application.Enums;
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
        WiFiSecurityType type = WiFiSecurityType.WPA,
        Bitmap? bitmap = null)
    {
        var content = string.Concat("WIFI:S:", ssid, ";T:", type.ToString(), ";P:", password, ";;");

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new SvgQRCode(qrCodeData))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(20, Color.Black, Color.White, true, SizingMode.WidthHeightAttribute, ConvertToSvgLog(bitmap)));

        }
    }

    private static SvgLogo? ConvertToSvgLog(Bitmap? bitmap)
    {
        return bitmap is not null ? new SvgLogo(bitmap) : null;
    }
}

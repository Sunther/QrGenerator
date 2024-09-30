using QRCoder;
using QrGenerator.Application.Enums;
using QrGenerator.Disk;

namespace QrGenerator.Application.QrCodeCoders;

public class SvgQrCoder
{
    private readonly FileWriter _fileWriter;

    public SvgQrCoder()
    {
        _fileWriter = new FileWriter();
    }

    public void CreateBasicFile(string filePath, string content)
    {
        using (var qrCode = CreateCoder(content))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(20, System.Drawing.Color.Black, System.Drawing.Color.White, true));
        }
    }

    public void CreateWiFiFile(
        string ssid,
        string password,
        string filePath,
        WiFiSecurityType type = WiFiSecurityType.WPA)
    {
        var content = string.Concat("WIFI:S:", ssid, ";T:", type.ToString(), ";P:", password, ";;");

        using (var qrCode = CreateCoder(content))
        {
            _fileWriter.CreateFile(
                filePath,
                qrCode.GetGraphic(20, System.Drawing.Color.Black, System.Drawing.Color.White, true));
        }
    }

    private static SvgQRCode CreateCoder(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(
                                                content,
                                                QRCodeGenerator.ECCLevel.Q);
        return new SvgQRCode(qrCodeData);
    }
}

using System.Drawing;
using System.Drawing.Text;
using QRCoder;
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

        var img = AddCaption(bitmap, ssid);

        return new SvgLogo(img, fillLogoBackground: false);
    }

    private static Bitmap AddCaption(Bitmap bitmap, string? ssid, int textHeight = 50)
    {
        if (string.IsNullOrEmpty(ssid))
        {
            return bitmap;
        }

        // Create a new bitmap with a size of loaded image + rectangle for caption
        var img = new Bitmap(bitmap.Width, bitmap.Height + textHeight);
        var graphics = Graphics.FromImage(img);

        // Draw the loaded image on newly created image
        graphics.DrawImage(bitmap, 0, 0);

        // Draw a rectangle for caption box
        var rectangle = new Rectangle(0, bitmap.Height, bitmap.Width, textHeight);
        graphics.DrawRectangle(
                    new Pen(Color.White, 2),
                    rectangle);
        graphics.FillRectangle(
                    new SolidBrush(Color.White),
                    rectangle);

        // Draw text
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.DrawString(
                    ssid,
                    new Font("Arial", 30, FontStyle.Regular),
                    brush: new SolidBrush(Color.Black),
                    layoutRectangle: rectangle,
                    new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });

        return img;
    }
}

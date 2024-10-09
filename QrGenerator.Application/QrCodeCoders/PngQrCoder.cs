using QRCoder;
using QrGenerator.Application.Extensions;
using System.Drawing;

namespace QrGenerator.Application.QrCodeCoders;

public class PngQrCoder
{
    public void CreateWiFiQrFile(
        string ssid,
        string password,
        string filePath,
        PayloadGenerator.WiFi.Authentication type = PayloadGenerator.WiFi.Authentication.WPA,
        Bitmap? bitmap = null)
    {
        var wifiPayload = new PayloadGenerator.WiFi(ssid, password, PayloadGenerator.WiFi.Authentication.WPA);

        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new QRCode(qrCodeData))
        {
            var qrCodeAsBitmapByteArr = qrCode.GetGraphic(20, Color.Black, Color.White, icon: bitmap?.AddCaption(ssid));

            qrCodeAsBitmapByteArr.Save(filePath);
        }
    }
}

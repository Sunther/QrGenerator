using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;
using QrGenerator.Desktop.Resources.LanguageResources;
using System.Drawing;
using Windows.Globalization;

namespace QrGenerator.Desktop.ViewModels;

internal partial class WiFiQR : ObservableObject
{
    [ObservableProperty]
    private Bitmap? _bitmapQrPreview;
    [ObservableProperty]
    private string? _imagePath;

    private readonly SvgQrCoder _svgCode;

    public string? Ssid { get; set; }
    public string? Password { get; set; }
    public IList<string> AuthenticationPickerSource { get; }
    public string SelectedAuthenticationType { get; set; }
    public IAsyncRelayCommand DisplayWiFiImageCommand { get; }
    public IAsyncRelayCommand SaveWiFiImageCommand { get; }

    public WiFiQR()
    {
        _svgCode = new SvgQrCoder();

        AuthenticationPickerSource = new List<string>()
            {
                "WEP",
                "WPA",
                "nopass",
                "WPA2"
            };

        DisplayWiFiImageCommand = new AsyncRelayCommand(DisplayWifiImage);
        SaveWiFiImageCommand = new AsyncRelayCommand(SaveSvgWifiImage);

        SelectedAuthenticationType = "WPA";
    }

    private async Task DisplayWifiImage()
    {
        if (CheckParameters())
        {
            return;
        }

        await Task.Run(() =>
        {
            BitmapQrPreview = PngQrCoder.GetWiFiQr(
                    Ssid,
                    Password,
                    SelectedAuthenticationType,
                    ImagePath);
        });
    }

    private async Task SaveSvgWifiImage()
    {
        if (CheckParameters())
        {
            return;
        }

        await Task.Run(() =>
        {
            _svgCode.CreateWiFiFile(
                Ssid,
                Password,
                authType: SelectedAuthenticationType,
                imagePath: ImagePath);
        });
    }

    private bool CheckParameters()
    {
        var listErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(Ssid))
        {
            listErrors.Add(LanguageLiterals.SsidNotEmpty);
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            listErrors.Add(LanguageLiterals.PasswordNotEmpty);
        }

        if (listErrors.Count > 0)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                WeakReferenceMessenger.Default.Send(string.Join(Environment.NewLine, listErrors));
            });
        }

        return listErrors.Count > 0;
    }
}

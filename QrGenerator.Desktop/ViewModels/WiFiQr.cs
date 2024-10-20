using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;
using System.Drawing;

namespace QrGenerator.Desktop.ViewModels;

internal partial class WiFiQR : ObservableObject
{
    private readonly string DefaultPathSvg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "QR.svg");

    [ObservableProperty]
    private ImageSource? _previewLogo;
    [ObservableProperty]
    private string? _imageName;
    [ObservableProperty]
    private Bitmap? _bitmapQrPreview;

    private readonly SvgQrCoder _svgCode;
    private string? _imagePath;

    public string? Ssid { get; set; }
    public string? Password { get; set; }
    public IList<string> AuthenticationPickerSource { get; }
    public string SelectedAuthenticationType { get; set; }
    public IAsyncRelayCommand SelectImageCommand { get; }
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

        SelectImageCommand = new AsyncRelayCommand(SelectImage);
        DisplayWiFiImageCommand = new AsyncRelayCommand(DisplayWifiImage);
        SaveWiFiImageCommand = new AsyncRelayCommand(SaveSvgWifiImage);

        SelectedAuthenticationType = "WPA";
    }

    private async Task SelectImage()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Select an image"
        });

        if (result is not null)
        {
            _imagePath = result.FullPath;
            ImageName = Path.GetFileName(_imagePath);

            await Task.Run(() =>
            {
                using (var ms = new MemoryStream())
                using (var bitmap = new Bitmap(_imagePath))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());
                    PreviewLogo = ImageSource.FromStream(() => stream);
                }
            });
        }
    }

    private async Task DisplayWifiImage()
    {
        await Task.Run(() =>
        {
            if (CheckParameters())
            {
                return;
            }

            BitmapQrPreview = PngQrCoder.GetWiFiQr(
                    Ssid,
                    Password,
                    SelectedAuthenticationType,
                    _imagePath);
        });
    }

    private async Task SaveSvgWifiImage()
    {
        await Task.Run(() =>
        {
            if (CheckParameters())
            {
                return;
            }

            _svgCode.CreateWiFiFile(
                Ssid,
                Password,
                DefaultPathSvg,
                SelectedAuthenticationType,
                _imagePath);
        });
    }

    private bool CheckParameters()
    {
        var listErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(Ssid))
        {
            listErrors.Add("SSID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            listErrors.Add("Password cannot be empty.");
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

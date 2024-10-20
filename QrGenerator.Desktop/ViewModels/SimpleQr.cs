using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;

namespace QrGenerator.Desktop.ViewModels;

internal partial class SimpleQr : ObservableObject
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

    public string? Content { get; set; }
    public bool IsUrlChecked { get; set; }
    public IAsyncRelayCommand SelectImageCommand { get; }
    public IAsyncRelayCommand DisplayWiFiImageCommand { get; }
    public IAsyncRelayCommand SaveWiFiImageCommand { get; }

    public SimpleQr()
    {
        _svgCode = new SvgQrCoder();

        SelectImageCommand = new AsyncRelayCommand(SelectImage);
        DisplayWiFiImageCommand = new AsyncRelayCommand(DisplayImage);
        SaveWiFiImageCommand = new AsyncRelayCommand(GenerateSvgImage);
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

    private async Task DisplayImage()
    {
        await Task.Run(() =>
        {
            if (CheckParameters())
            {
                return;
            }

            BitmapQrPreview = IsUrlChecked ?
                PngQrCoder.GetUrlQr(Content, _imagePath) :
                PngQrCoder.GetQr(Content, _imagePath);
        });
    }

    private async Task GenerateSvgImage()
    {
        await Task.Run(() =>
        {
            if (CheckParameters())
            {
                return;
            }

            _svgCode.CreateBasicFile(
                Content,
                DefaultPathSvg,
                _imagePath);
        });
    }

    private bool CheckParameters()
    {
        var listErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(Content))
        {
            listErrors.Add("Content cannot be empty.");
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

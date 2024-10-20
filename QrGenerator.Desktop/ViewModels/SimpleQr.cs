using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;

namespace QrGenerator.Desktop.ViewModels;

internal partial class SimpleQr : ObservableObject
{
    [ObservableProperty]
    private Bitmap? _bitmapQrPreview;
    [ObservableProperty]
    private string? _imagePath;

    private readonly SvgQrCoder _svgCode;

    public string? Content { get; set; }
    public bool IsUrlChecked { get; set; }
    public IAsyncRelayCommand DisplayWiFiImageCommand { get; }
    public IAsyncRelayCommand SaveWiFiImageCommand { get; }

    public SimpleQr()
    {
        _svgCode = new SvgQrCoder();

        DisplayWiFiImageCommand = new AsyncRelayCommand(DisplayImage);
        SaveWiFiImageCommand = new AsyncRelayCommand(GenerateSvgImage);
    }

    private async Task DisplayImage()
    {
        if (CheckParameters())
        {
            return;
        }

        await Task.Run(() =>
        {
            BitmapQrPreview = IsUrlChecked ?
                PngQrCoder.GetUrlQr(Content, ImagePath) :
                PngQrCoder.GetQr(Content, ImagePath);
        });
    }

    private async Task GenerateSvgImage()
    {
        if (CheckParameters())
        {
            return;
        }

        await Task.Run(() =>
        {
            if (IsUrlChecked)
            {
                _svgCode.CreateUrlFile(Content, imagePath: ImagePath);
            }
            else
            {
                _svgCode.CreateBasicFile(Content, imagePath: ImagePath);
            }
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

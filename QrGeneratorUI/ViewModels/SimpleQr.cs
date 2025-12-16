using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;
using QrGeneratorUI.Resources.LanguageResources;

namespace QrGeneratorUI.ViewModels;

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
            try
            {
                BitmapQrPreview = IsUrlChecked ?
                    PngQrCoder.GetUrlQr(Content, ImagePath) :
                    PngQrCoder.GetQr(Content, ImagePath);
            }
            catch (Exception ex)
            {
                // Display a checkbox with the error message
            }
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
            try
            {
                if (IsUrlChecked)
                {
                    _svgCode.CreateUrlFile(Content, imagePath: ImagePath);
                }
                else
                {
                    _svgCode.CreateBasicFile(Content, imagePath: ImagePath);
                }
            }
            catch (Exception ex)
            {
                // Display a checkbox with the error message
            }
        });
    }

    private bool CheckParameters()
    {
        var listErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(Content))
        {
            listErrors.Add(LanguageLiterals.ContentNotEmpy);
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

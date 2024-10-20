using CommunityToolkit.Mvvm.Input;
using QrGenerator.Disk;
using System.Drawing;

namespace QrGenerator.Desktop.ContentViews;

public partial class DisplayImage : ContentView
{
    private readonly string DefaultPathSvg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "QR.svg");

    public static readonly BindableProperty DisplayCommandProperty = BindableProperty.Create(nameof(DisplayCommand), typeof(IAsyncRelayCommand), typeof(DisplayImage), null);
    public static readonly BindableProperty SaveCommandProperty = BindableProperty.Create(nameof(SaveCommand), typeof(IAsyncRelayCommand), typeof(DisplayImage), null);
    public static readonly BindableProperty BitmapQrProperty = BindableProperty.Create(nameof(BitmapQr), typeof(Bitmap), typeof(DisplayImage), null);

    public IAsyncRelayCommand DisplayCommand
    {
        get => (IAsyncRelayCommand)GetValue(DisplayCommandProperty);
        set => SetValue(DisplayCommandProperty, value);
    }
    public IAsyncRelayCommand SaveCommand
    {
        get => (IAsyncRelayCommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }
    public Bitmap BitmapQr
    {
        get => (Bitmap)GetValue(BitmapQrProperty);
        set => SetValue(BitmapQrProperty, value);
    }

    public DisplayImage()
    {
        //global::Microsoft.Maui.Controls.Xaml.Extensions.LoadFromXaml(this, typeof(DisplayImage));
        InitializeComponent();
    }

    private async void DisplayImageButton_Clicked(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            LoadingImageActivityIndicator.IsRunning = true;
            LoadingImageActivityIndicator.IsVisible = true;
        });

        await DisplayCommand.ExecuteAsync(null);

        if (BitmapQr is not null)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream())
                {
                    BitmapQr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());

                    Dispatcher.Dispatch(() =>
                    {
                        QrImage.Source = ImageSource.FromStream(() => stream);
                    });
                }
            });
        }

        Dispatcher.Dispatch(() =>
        {
            LoadingImageActivityIndicator.IsRunning = false;
            LoadingImageActivityIndicator.IsVisible = false;
        });
    }

    private async void SaveSvgImageButton_Clicked(object sender, EventArgs e)
    {
        await SaveCommand.ExecuteAsync(null);

        ExplorerManagement.OpenFolderContainingFile(DefaultPathSvg);
    }
}
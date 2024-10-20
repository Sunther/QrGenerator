using CommunityToolkit.Mvvm.Input;
using QrGenerator.Disk;
using System.Drawing;

namespace QrGenerator.Desktop.ContentViews;

public partial class DisplayImage : ContentView
{
    private readonly string DefaultPathSvg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "QR.svg");
    private readonly string DefaultPathPngTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "QR.png");

    public static readonly BindableProperty DisplayCommandProperty = BindableProperty.Create(nameof(DisplayCommand), typeof(IAsyncRelayCommand), typeof(DisplayImage), null);
    public static readonly BindableProperty SaveCommandProperty = BindableProperty.Create(nameof(SaveCommand), typeof(IAsyncRelayCommand), typeof(DisplayImage), null);

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

        await Task.Run(() =>
        {
            if (File.Exists(DefaultPathPngTemp))
            {
                using (var bitmapQr = new Bitmap(DefaultPathPngTemp))
                using (var ms = new MemoryStream())
                {
                    bitmapQr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());

                    Dispatcher.Dispatch(() =>
                    {
                        QrImage.Source = ImageSource.FromStream(() => stream);
                    });
                }

                File.Delete(DefaultPathPngTemp);
            }
        });

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
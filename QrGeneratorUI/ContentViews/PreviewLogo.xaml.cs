using System.Drawing;

namespace QrGeneratorUI.ContentViews;

public partial class PreviewLogo : ContentView
{
    public static readonly BindableProperty LogoPathProperty = BindableProperty.Create(nameof(LogoPath), typeof(string), typeof(DisplayImage), null);

    public string LogoPath
    {
        get => (string)GetValue(LogoPathProperty);
        set => SetValue(LogoPathProperty, value);
    }

    public PreviewLogo()
    {
        InitializeComponent();
    }

    private async void ImageSelectorButton_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Select an image"
        });

        if (result is not null)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream())
                using (var bitmap = new Bitmap(result.FullPath))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());
                    Dispatcher.Dispatch(() =>
                    {
                        LogoPath = result.FullPath;
                        LogoNameLabel.Text = Path.GetFileName(LogoPath);
                        LogoPreview.Source = ImageSource.FromStream(() => stream);
                    });
                }
            });
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (HasLogoCheckbox.IsChecked)
        {
            ImageSelectorButton.IsVisible = true;
            LogoNameLabel.IsVisible = true;
            LogoPreview.IsVisible = true;
        }
        else
        {
            ImageSelectorButton.IsVisible = false;
            LogoNameLabel.IsVisible = false;
            LogoPreview.IsVisible = false;
        }
    }
}
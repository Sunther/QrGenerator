using System.Drawing;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QrGenerator.Application.QrCodeCoders;
using QrGenerator.Disk;

namespace QrGenerator.Desktop.ViewModels
{
    internal partial class Main : ObservableObject
    {
        private const string DefaultPathSvg = "C:\\TEMP\\QR.svg";

        [ObservableProperty]
        private ImageSource? _imageQr;
        [ObservableProperty]
        private ImageSource? _previewLogo;
        [ObservableProperty]
        private string? _imagePath;

        private readonly SvgQrCoder SvgCode;
        private readonly PngQrCoder PngCode;

        public string? Ssid { get; set; }
        public string? Password { get; set; }
        public IList<string> AuthenticationPickerSource { get; }
        public string SelectedAuthenticationType { get; set; }
        public ICommand SelectImageCommand { get; }
        public ICommand VisualizeImageCommand { get; }
        public ICommand GenerateImageCommand { get; }

        public Main()
        {
            SvgCode = new SvgQrCoder();
            PngCode = new PngQrCoder();

            AuthenticationPickerSource = new List<string>()
            {
                "WEP",
                "WPA",
                "nopass",
                "WPA2"
            };

            SelectImageCommand = new AsyncRelayCommand(SelectImage);
            VisualizeImageCommand = new RelayCommand(VisualizeImage);
            GenerateImageCommand = new RelayCommand(GenerateSvgImage);

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
                ImagePath = result.FullPath;

                using (var ms = new MemoryStream())
                using (var bitmap = new Bitmap(ImagePath))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());
                    PreviewLogo = ImageSource.FromStream(() => stream);
                }
            }
        }

        private void VisualizeImage()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Ssid);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Password);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(ImagePath);

            var bitmapQr = PngCode.GetWiFiQr(
                Ssid,
                Password,
                SelectedAuthenticationType,
                bitmap: new Bitmap(ImagePath));

            using (var ms = new MemoryStream())
            {
                bitmapQr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                var stream = new MemoryStream(ms.ToArray());
                ImageQr = ImageSource.FromStream(() => stream);
            }
        }

        private void GenerateSvgImage()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Ssid);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Password);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(ImagePath);

            SvgCode.CreateWiFiFile(
                Ssid,
                Password,
                DefaultPathSvg,
                bitmap: new Bitmap(ImagePath));

            ExplorerManagement.OpenFolderContainingFile(DefaultPathSvg);
        }
    }
}

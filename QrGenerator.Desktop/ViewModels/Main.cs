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
        private const string DefaultPathPng = "C:\\TEMP\\QR.png";
        private const string DefaultPathSvg = "C:\\TEMP\\QR.svg";

        [ObservableProperty]
        private ImageSource? _imageQr;
        private readonly SvgQrCoder SvgCode;
        private readonly PngQrCoder PngCode;
        private string? ImagePath;

        public string? Ssid { get; set; }
        public string? Password { get; set; }
        public IList<string> AuthenticationPickerSource { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand VisualizeImageCommand { get; }
        public ICommand GenerateImageCommand { get; }

        public Main()
        {
            SelectImageCommand = new AsyncRelayCommand(SelectImage);
            VisualizeImageCommand = new RelayCommand(VisualizeImage);
            GenerateImageCommand = new RelayCommand(GenerateSvgImage);
            SvgCode = new SvgQrCoder();
            PngCode = new PngQrCoder();
            AuthenticationPickerSource = new List<string>()
            {
                "WPA",
            };
        }

        private async Task SelectImage()
        {
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select an Image"
            };

            var result = await FilePicker.Default.PickAsync(options);

            if (result != null)
            {
                ImagePath = result.FullPath;
            }
        }

        private void VisualizeImage()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Ssid);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Password);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(ImagePath);

            PngCode.CreateWiFiQrFile(
                Ssid,
                Password,
                DefaultPathPng,
                bitmap: new Bitmap(ImagePath));

            ImageQr = ImageSource.FromFile(DefaultPathPng);
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

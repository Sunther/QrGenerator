using System.Drawing;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QrGenerator.Application.Factories;
using QrGenerator.Application.QrCodeCoders;

namespace QrGenerator.Desktop.ViewModels
{
    internal partial class Main : ObservableObject
    {
        private const string DefaultPath = "C:\\TEMP\\QR.svg";

        [ObservableProperty]
        private ImageSource? _imageQr;
        private readonly SvgQrCoder SvgCode;
        private string? ImagePath;

        public string? Ssid { get; set; }
        public string? Password { get; set; }
        public IList<string> AuthenticationPickerSource { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand GenerateImageCommand { get; }

        public Main()
        {
            SelectImageCommand = new AsyncRelayCommand(SelectImage);
            GenerateImageCommand = new RelayCommand(GenerateImage);
            SvgCode = QrGeneratorFactory.GetQrCodeGenerator();
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

        private void GenerateImage()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Ssid);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(Password);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(ImagePath);

            SvgCode.CreateWiFiFile(
                Ssid,
                Password,
                DefaultPath,
                bitmap: new Bitmap(ImagePath));

            // Convert to PNG to display in the UI
            ImageQr = ImageSource.FromFile(DefaultPath);
        }
    }
}

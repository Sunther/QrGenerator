using System.Drawing;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Application.QrCodeCoders;
using QrGenerator.Disk;

namespace QrGenerator.Desktop.ViewModels
{
    internal partial class Main : ObservableObject
    {
        private readonly string DefaultPathSvg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "QR.svg");

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
            if (CheckParameters())
            {
                return;
            }

            var bitmapQr = PngCode.GetWiFiQr(
                Ssid,
                Password,
                SelectedAuthenticationType,
                ImagePath);

            using (var ms = new MemoryStream())
            {
                bitmapQr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                var stream = new MemoryStream(ms.ToArray());
                ImageQr = ImageSource.FromStream(() => stream);
            }
        }

        private void GenerateSvgImage()
        {
            if (CheckParameters())
            {
                return;
            }

            SvgCode.CreateWiFiFile(
                Ssid,
                Password,
                DefaultPathSvg,
                SelectedAuthenticationType,
                ImagePath);

            ExplorerManagement.OpenFolderContainingFile(DefaultPathSvg);
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
                WeakReferenceMessenger.Default.Send(string.Join(Environment.NewLine, listErrors));
            }

            return listErrors.Count > 0;
        }
    }
}

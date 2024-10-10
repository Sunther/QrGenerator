﻿using System.Drawing;
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
        private ImageSource? _imageSourceQr;
        [ObservableProperty]
        private ImageSource? _previewLogo;
        [ObservableProperty]
        private string? _imageName;
        [ObservableProperty]
        private bool _isImageLoading;

        private readonly SvgQrCoder _svgCode;
        private readonly PngQrCoder _pngCode;
        private string? _imagePath;

        public string? Ssid { get; set; }
        public string? Password { get; set; }
        public IList<string> AuthenticationPickerSource { get; }
        public string SelectedAuthenticationType { get; set; }
        public ICommand SelectImageCommand { get; }
        public ICommand DisplayImageCommand { get; }
        public ICommand GenerateImageCommand { get; }

        public Main()
        {
            IsImageLoading = false;

            _svgCode = new SvgQrCoder();
            _pngCode = new PngQrCoder();

            AuthenticationPickerSource = new List<string>()
            {
                "WEP",
                "WPA",
                "nopass",
                "WPA2"
            };

            SelectImageCommand = new AsyncRelayCommand(SelectImage);
            DisplayImageCommand = new AsyncRelayCommand(DisplayImage);
            GenerateImageCommand = new AsyncRelayCommand(GenerateSvgImage);

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

                IsImageLoading = true;

                var bitmapQr = _pngCode.GetWiFiQr(
                    Ssid,
                    Password,
                    SelectedAuthenticationType,
                    _imagePath);

                using (var ms = new MemoryStream())
                {
                    bitmapQr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    var stream = new MemoryStream(ms.ToArray());
                    ImageSourceQr = ImageSource.FromStream(() => stream);
                }

                IsImageLoading = false;
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

                _svgCode.CreateWiFiFile(
                    Ssid,
                    Password,
                    DefaultPathSvg,
                    SelectedAuthenticationType,
                    _imagePath);

                ExplorerManagement.OpenFolderContainingFile(DefaultPathSvg);
            });
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

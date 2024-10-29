using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using QrGenerator.Desktop.Resources.LanguageResources;

namespace QrGenerator.Desktop
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();

            LanguageLiterals.Culture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture;
            MainPage = new AppShell();

            WeakReferenceMessenger.Default.Register<CultureInfo>(this, (recipient, culture) =>
            {
                LanguageLiterals.Culture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
                MainPage = new AppShell();
            });

            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                MainPage.DisplayAlert("Error", m, "OK");
            });
        }
    }
}

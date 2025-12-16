using System.Globalization;
using QrGeneratorUI.Resources.LanguageResources;

namespace QrGeneratorUI;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();

        LanguageLiterals.Culture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture;
        MainPage = new AppShell();

        //WeakReferenceMessenger.Default.Register<CultureInfo>(this, (recipient, culture) =>
        //{
        //    LanguageLiterals.Culture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
        //    MainPage = new AppShell();
        //});

        //WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        //{
        //    MainPage.DisplayAlert("Error", m, "OK");
        //});
    }
}
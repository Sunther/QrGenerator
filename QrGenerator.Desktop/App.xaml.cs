using System.Globalization;
using QrGenerator.Desktop.Resources.LanguageResources;

namespace QrGenerator.Desktop
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            LanguageLiterals.Culture = CultureInfo.CurrentCulture;
        }
    }
}

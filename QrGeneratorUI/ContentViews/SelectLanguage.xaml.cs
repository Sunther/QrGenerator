using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using QrGeneratorUI.Resources.LanguageResources;

namespace QrGeneratorUI.ContentViews;

public partial class SelectLanguage : ContentView
{
    private static readonly CultureInfo _cultureSpanish = new("es-ES");
    private static readonly CultureInfo _cultureAmerican = new("en-US");

    public SelectLanguage()
    {
        InitializeComponent();

        switch (CultureInfo.CurrentUICulture.Name)
        {
            case "es-ES":
                SpanishImageButton.IsVisible = true;
                break;
            case "en-US":
                AmericanImageButton.IsVisible = true;
                break;
        }
    }

    private void ChangeToSpanish(object sender, EventArgs e)
    {
        ChangeLanguage(_cultureSpanish);
    }

    private void ChangeToEnglish(object sender, EventArgs e)
    {
        ChangeLanguage(_cultureAmerican);
    }

    private void ChangeLanguage(CultureInfo culture)
    {
        LanguageLiterals.Culture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
        WeakReferenceMessenger.Default.Send(culture);

        SpanishImageButton.IsVisible = !SpanishImageButton.IsVisible;
        AmericanImageButton.IsVisible = !AmericanImageButton.IsVisible;
    }
}
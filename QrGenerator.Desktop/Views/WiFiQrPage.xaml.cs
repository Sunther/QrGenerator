using CommunityToolkit.Mvvm.Messaging;

namespace QrGenerator.Desktop.Views;

public partial class WiFiQrPage : ContentPage
{
	public WiFiQrPage()
	{
		InitializeComponent();

        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            DisplayAlert("Error", m, "OK");
        });
    }
}
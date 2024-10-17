using CommunityToolkit.Mvvm.Messaging;

namespace QrGenerator.Desktop.Views;

public partial class SimpleQrPage : ContentPage
{
	public SimpleQrPage()
	{
		InitializeComponent();

        WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            DisplayAlert("Error", m, "OK");
        });
    }
}
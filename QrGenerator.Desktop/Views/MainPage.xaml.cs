using CommunityToolkit.Mvvm.Messaging;

namespace QrGenerator.Desktop.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                DisplayAlert("Error", m, "OK");
            });
        }
    }
}
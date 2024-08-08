using SitHanumanApp.Services;
using System.Net.Http.Headers;
using ZXing.Net.Maui;

namespace SitHanumanApp
{
    public partial class FeatureListPage : ContentPage
    {
        public FeatureListPage()
        {
            InitializeComponent();
        }

        private async void OnQRCodeFeatureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRCodePage());
        }
    }
}

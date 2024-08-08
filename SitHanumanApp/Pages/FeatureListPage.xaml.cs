using SitHanumanApp.Services;

namespace SitHanumanApp
{
    public partial class FeatureListPage : ContentPage
    {
        private readonly ApiService _apiService;
        public FeatureListPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        private async void OnQRCodeFeatureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRCodeScanPage(_apiService));
        }
    }
}

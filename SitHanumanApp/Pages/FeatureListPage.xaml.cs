using SitHanumanApp.Services;

namespace SitHanumanApp
{
    public partial class FeatureListPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly TokenService _tokenService;
        public FeatureListPage(ApiService apiService, TokenService tokenService)
        {
            InitializeComponent();
            _apiService = apiService;
            _tokenService = tokenService;
        }

        private async void OnQRCodeFeatureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRCodeScanPage(_apiService));
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            // Invalida i token e cancella le informazioni di login
            await _tokenService.LogoutAsync();
            Application.Current.MainPage = new MainPage();
        }
    }
}

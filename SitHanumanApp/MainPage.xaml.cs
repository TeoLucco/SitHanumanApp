using Microsoft.Extensions.Configuration;
using SitHanumanApp.Services;

namespace SitHanumanApp
{
    public partial class MainPage : ContentPage
    {
        private readonly TokenService _tokenService;

        // Questo costruttore è utilizzato dal designer e dalle risorse XAML
        public MainPage()
        {
            InitializeComponent();

            // Risolvi i servizi dal ServiceProvider
            _tokenService = (Application.Current as App)?.ServiceProvider.GetRequiredService<TokenService>();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            var loginResult = await _tokenService.LoginAsync(username, password);
            if (loginResult != null)
            {
                // Salva i token per le chiamate successive
                Preferences.Set("accessToken", loginResult.AccessToken);
                Preferences.Set("refreshToken", loginResult.RefreshToken);
            }
        }
    }

}

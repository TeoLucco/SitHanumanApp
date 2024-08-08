using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SitHanumanApp.Services;

namespace SitHanumanApp
{
    public partial class MainPage : ContentPage
    {
        private readonly TokenService _tokenService;
        private readonly ApiService _apiService;

        public MainPage()
        {
            InitializeComponent();

            _tokenService = (Application.Current as App)?.ServiceProvider.GetRequiredService<TokenService>();
            _apiService = (Application.Current as App)?.ServiceProvider.GetRequiredService<ApiService>();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;
            // Mostra il loader
            Loader.IsRunning = true;
            Loader.IsVisible = true;
            LoginButton.IsEnabled = false;

            try
            {
                var loginResult = await _tokenService.LoginAsync(username, password);
                if (loginResult != null)
                {
                    await Navigation.PushAsync(new FeatureListPage(_apiService));
                }
                else
                {
                    await DisplayAlert("Errore", "Login fallito: username o password non validi.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", ex.Message, "OK");
            }
            finally
            {
                // Nascondi il loader
                Loader.IsRunning = false;
                Loader.IsVisible = false;
                LoginButton.IsEnabled = true;
            }
        }
    }
}

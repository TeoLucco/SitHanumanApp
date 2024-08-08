using Microsoft.Extensions.Configuration;
using SitHanumanApp.Services;

namespace SitHanumanApp
{
    public partial class MainPage : ContentPage
    {
        private readonly TokenService _tokenService;

        public MainPage()
        {
            InitializeComponent();

            _tokenService = (Application.Current as App)?.ServiceProvider.GetRequiredService<TokenService>();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            try
            {
                var loginResult = await _tokenService.LoginAsync(username, password);
                if (loginResult != null)
                {
                    await Navigation.PushAsync(new FeatureListPage());
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
        }
    }
}

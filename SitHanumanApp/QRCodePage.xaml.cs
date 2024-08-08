using SitHanumanApp.Services;
using System.Net.Http.Headers;
using ZXing.Net.Maui;

namespace SitHanumanApp
{
    public partial class QRCodePage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        public QRCodePage()
        {
            InitializeComponent();
            cameraView.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.All,
                AutoRotate = true,
                Multiple = true
            };
            _httpClient = new HttpClient();
            _tokenService = (Application.Current as App)?.ServiceProvider.GetRequiredService<TokenService>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            cameraView.IsDetecting = true; // Riattiva la scansione quando la pagina riappare
        }

        private async void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            cameraView.IsDetecting = false;
            foreach (var barcode in e.Results)
            {
                var value = barcode.Value;
                if (value.StartsWith("https://sithanumanmng.azurewebsites.net/api/entrances_subscriptions/add_entrance/"))
                {
                    try
                    {
                        var accessToken = await _tokenService.GetAccessTokenAsync();
                        if (accessToken != null)
                        {
                            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                            var response = await _httpClient.PostAsync(value, null);

                            if (response.IsSuccessStatusCode)
                            {
                                var responseData = await response.Content.ReadAsStringAsync();
                                await MainThread.InvokeOnMainThreadAsync(async () =>
                                {
                                    await Navigation.PushAsync(new EntrancesQRCodeSuccessPage(responseData));
                                });
                            }
                            else
                            {
                                var errorData = await response.Content.ReadAsStringAsync();
                                await MainThread.InvokeOnMainThreadAsync(async () =>
                                {
                                    await Navigation.PushAsync(new EntrancesQRCodeFailPage(errorData));
                                });
                            }
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Navigation.PushAsync(new EntrancesQRCodeFailPage("Token di accesso non valido."));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Navigation.PushAsync(new EntrancesQRCodeFailPage(ex.Message));
                        });
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Navigation.PushAsync(new EntrancesQRCodeFailPage("QR code non valido."));
                    });
                }
                return;
            }
        }
    }
}

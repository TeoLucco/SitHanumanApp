using SitHanumanApp.Models;
using SitHanumanApp.Services;
using ZXing.Net.Maui;

namespace SitHanumanApp
{
    public partial class QRCodeScanPage : ContentPage
    {
        private readonly ApiService _apiService;


        public QRCodeScanPage(ApiService apiService)
        {
            _apiService = apiService;
            InitializeComponent();
            cameraView.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.All,
                AutoRotate = true,
                Multiple = true
            };
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
                        var apiResponse = await _apiService.PostAsync<AddEntranceApiResponse>(value, null);


                        if (apiResponse == null)
                        {
                            throw new Exception("Unexpected Response");
                        }

                        if (apiResponse.Success)
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Navigation.PushAsync(new QRCodeResultPage(apiResponse.Success, apiResponse.Data, apiResponse.Error));
                            });
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Navigation.PushAsync(new QRCodeResultPage(apiResponse.Success, apiResponse.Data, apiResponse.Error));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Navigation.PushAsync(new QRCodeExceptionPage(ex.Message));
                        });
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Navigation.PushAsync(new QRCodeExceptionPage("QR code non valido."));
                    });
                }
                return;
            }
        }
    }
}

namespace SitHanumanApp
{
    public partial class EntrancesQRCodeSuccessPage : ContentPage
    {
        public EntrancesQRCodeSuccessPage(string responseData)
        {
            InitializeComponent();
            ResponseLabel.Text = responseData;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();  // Torna alla lista delle funzionalità
        }
    }
}

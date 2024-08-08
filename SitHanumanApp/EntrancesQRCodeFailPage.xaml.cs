namespace SitHanumanApp
{
    public partial class EntrancesQRCodeFailPage : ContentPage
    {
        public EntrancesQRCodeFailPage(string errorData)
        {
            InitializeComponent();
            ErrorLabel.Text = errorData;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();  // Torna alla lista delle funzionalità
        }
    }
}

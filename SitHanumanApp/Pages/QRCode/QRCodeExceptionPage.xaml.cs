namespace SitHanumanApp
{
    public partial class QRCodeExceptionPage : ContentPage
    {
        public QRCodeExceptionPage(string errorData)
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

using SitHanumanApp.Models;

namespace SitHanumanApp
{
    public partial class QRCodeResultPage : ContentPage
    {
        public QRCodeResultPage(bool success, EntranceData entranceData, string? errorMessage)
        {
            InitializeComponent(); 

            if (success)
            {
                if(entranceData.EntrancesDone < entranceData.EntrancesNumber && entranceData.PaymentCompleted)
                {
                    this.BackgroundColor = Colors.Green;
                    MessageLabel.Text = "Ingresso aggiunto con successo!";
                }
                else
                {
                    this.BackgroundColor= Colors.Yellow;
                    MessageLabel.TextColor = Colors.Black;
                    MemberLabel.TextColor = Colors.Black;
                    Member.TextColor = Colors.Black;
                    CourseLabel.TextColor = Colors.Black;
                    Course.TextColor = Colors.Black;
                    EntrancesLabel.TextColor = Colors.Black; 
                    Entrances.TextColor = Colors.Black;
                    PaymentStatusLabel.TextColor = Colors.Black; 
                    PaymentStatus.TextColor = Colors.Black; 
                    if (entranceData!.EntrancesDone == entranceData.EntrancesNumber && !entranceData.PaymentCompleted)
                    {
                        MessageLabel.Text = "Ultimo Accesso e abbonamento non totalmente pagato!";
                    }
                    if (entranceData!.EntrancesDone == entranceData.EntrancesNumber)
                    {
                        MessageLabel.Text = "Ultimo Accesso!";
                    }
                    if (!entranceData.PaymentCompleted)
                    {
                        MessageLabel.Text = "Ingresso aggiunto con successo, ma abbonamento non totalmente pagato!";
                    }
                }
            }
            else
            {
                this.BackgroundColor = Colors.Red;
                MessageLabel.Text = errorMessage;
            }

            Entrances.Text = $"{entranceData!.EntrancesDone} di {entranceData.EntrancesNumber}";

            // Imposta il valore di Payment Status
            if (entranceData.PaymentCompleted)
            {
                PaymentStatus.Text = "Pagamento completato!";
            }
            else
            {
                var feePaid = entranceData.FeePaid.ToString("F2");
                var price = entranceData.Price.ToString("F2");
                var remaining = (entranceData.Price - entranceData.FeePaid).ToString("F2");

                PaymentStatus.Text = $"Pagati {feePaid}€ su {price}€.\nMancano {remaining}€";
            }

            if (entranceData != null)
            {
                BindingContext = entranceData;
            }
        }

        private void OnBackToScannerClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}

using System.Windows;

namespace projektaflevering
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LogIndKnap_Click(object sender, RoutedEventArgs e)
        {
            string brugernavn = BrugernavnBox.Text.Trim();
            string adgangskode = AdgangskodeBox.Password;

            // Underviser-login
            if (brugernavn == "underviser" && adgangskode == "1234")
            {
                var mainWindow = new MainWindow(erUnderviser: true);
                mainWindow.Show();
                this.Close();
            }
            // Studerende-login
            else if (brugernavn == "studerende" && adgangskode == "1234")
            {
                var mainWindow = new MainWindow(erUnderviser: false);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                FejlTekst.Text = "Forkert brugernavn eller adgangskode.";
            }
        }
    }
}

using System.Windows;

namespace projektaflevering
{
    public partial class RedigerFlowWindow : Window
    {
        public string NyTitel       { get; private set; }
        public string NyBeskrivelse { get; private set; }

        public RedigerFlowWindow(Flow eksisterende)
        {
            InitializeComponent();

            // Udfyld med de eksisterende værdier
            TitelBox.Text       = eksisterende.Titel;
            BeskrivelseBox.Text = eksisterende.Beskrivelse;
        }

        private void GemKnap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitelBox.Text))
            {
                FejlTekst.Text = "Skriv venligst en titel.";
                return;
            }

            NyTitel       = TitelBox.Text.Trim();
            NyBeskrivelse = BeskrivelseBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}

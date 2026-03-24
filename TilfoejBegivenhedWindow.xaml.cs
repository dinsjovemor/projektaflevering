using System.Windows;

namespace projektaflevering
{
    public partial class TilfoejBegivenhedWindow : Window
    {
        public SkemaBlok NyBlok { get; private set; }

        public TilfoejBegivenhedWindow(string[] dage)
        {
            InitializeComponent();

            // Fyld dagene ind i dropdown
            foreach (var dag in dage)
                DagBox.Items.Add(dag);

            DagBox.SelectedIndex = 0;
        }

        private void TilfoejKnap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitelBox.Text))
            {
                FejlTekst.Text = "Skriv venligst en titel.";
                return;
            }

            if (!TryParseTid(StartBox.Text, out int startTime, out int startMinut))
            {
                FejlTekst.Text = "Ugyldig starttid. Brug formatet TT:MM.";
                return;
            }

            if (!TryParseTid(SlutBox.Text, out int slutTime, out int slutMinut))
            {
                FejlTekst.Text = "Ugyldig sluttid. Brug formatet TT:MM.";
                return;
            }

            if (slutTime * 60 + slutMinut <= startTime * 60 + startMinut)
            {
                FejlTekst.Text = "Sluttiden skal være efter starttiden.";
                return;
            }

            NyBlok = new SkemaBlok(DagBox.SelectedIndex, startTime, startMinut, slutTime, slutMinut, TitelBox.Text.Trim());
            DialogResult = true;
            Close();
        }

        // Hjælpemetode til at læse TT:MM
        private bool TryParseTid(string tekst, out int time, out int minut)
        {
            time = 0;
            minut = 0;

            var dele = tekst.Split(':');
            if (dele.Length != 2) return false;
            if (!int.TryParse(dele[0], out time)) return false;
            if (!int.TryParse(dele[1], out minut)) return false;
            if (time < 0 || time > 23) return false;
            if (minut < 0 || minut > 59) return false;

            return true;
        }
    }
}

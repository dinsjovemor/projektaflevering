using System;
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
            // Valider titel
            if (string.IsNullOrWhiteSpace(TitelBox.Text))
            {
                FejlTekst.Text = "Skriv venligst en titel.";
                return;
            }

            // Valider start-tid
            if (!TryParseTid(StartBox.Text, out int startH, out int startM))
            {
                FejlTekst.Text = "Ugyldig starttid. Brug formatet HH:MM.";
                return;
            }

            // Valider slut-tid
            if (!TryParseTid(SlutBox.Text, out int slutH, out int slutM))
            {
                FejlTekst.Text = "Ugyldig sluttid. Brug formatet HH:MM.";
                return;
            }

            // Tjek at slutten er efter starten
            if (slutH * 60 + slutM <= startH * 60 + startM)
            {
                FejlTekst.Text = "Sluttiden skal være efter starttiden.";
                return;
            }

            // Alt er OK – opret blok og luk vinduet
            NyBlok = new SkemaBlok(DagBox.SelectedIndex, startH, startM, slutH, slutM, TitelBox.Text.Trim());
            DialogResult = true;
            Close();
        }

        // Hjælpemetode til at læse HH:MM
        private bool TryParseTid(string tekst, out int hour, out int minute)
        {
            hour = 0;
            minute = 0;

            var dele = tekst.Split(':');
            if (dele.Length != 2) return false;
            if (!int.TryParse(dele[0], out hour)) return false;
            if (!int.TryParse(dele[1], out minute)) return false;
            if (hour < 0 || hour > 23) return false;
            if (minute < 0 || minute > 59) return false;

            return true;
        }
    }
}

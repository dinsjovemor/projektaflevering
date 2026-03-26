using System.Collections.Generic;
using System.Windows;

namespace projektaflevering
{
    public partial class RedigerBegivenhedWindow : Window
    {
        // Den nye SkemaBlok med de redigerede værdier
        public SkemaBlok NyBlok { get; private set; }

        private readonly string[] _dage;

        public RedigerBegivenhedWindow(SkemaBlok eksisterende, string[] dage, List<Flow> flows)
        {
            InitializeComponent();
            _dage = dage;

            // Fyld dag-dropdown med dagnavne
            foreach (var dag in dage)
                DagBox.Items.Add(dag);

            // Fyld flow-dropdown – første punkt er "Ingen"
            FlowBox.Items.Add("(Ingen)");
            foreach (var flow in flows)
                FlowBox.Items.Add(flow.Titel);

            // Udfyld felterne med de eksisterende værdier
            TitelBox.Text      = eksisterende.Titel;
            DagBox.SelectedIndex = eksisterende.DagIndeks;
            StartBox.Text      = $"{eksisterende.StartTime:D2}:{eksisterende.StartMinut:D2}";
            SlutBox.Text       = $"{eksisterende.SlutTime:D2}:{eksisterende.SlutMinut:D2}";

            // Vælg det tilknyttede flow i dropdown
            if (!string.IsNullOrEmpty(eksisterende.FlowTitel))
                FlowBox.SelectedItem = eksisterende.FlowTitel;
            else
                FlowBox.SelectedIndex = 0;
        }

        private void GemKnap_Click(object sender, RoutedEventArgs e)
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

            // Bestem valgt flow – "(Ingen)" gemmes som tom streng
            string valgtFlow = FlowBox.SelectedItem?.ToString();
            if (valgtFlow == "(Ingen)") valgtFlow = "";

            NyBlok = new SkemaBlok(
                DagBox.SelectedIndex,
                startTime, startMinut,
                slutTime, slutMinut,
                TitelBox.Text.Trim(),
                valgtFlow
            );

            DialogResult = true;
            Close();
        }

        private bool TryParseTid(string tekst, out int time, out int minut)
        {
            time = 0; minut = 0;
            var dele = tekst.Split(':');
            if (dele.Length != 2) return false;
            if (!int.TryParse(dele[0], out time))  return false;
            if (!int.TryParse(dele[1], out minut)) return false;
            if (time < 0 || time > 23)  return false;
            if (minut < 0 || minut > 59) return false;
            return true;
        }
    }
}

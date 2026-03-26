using System.Windows;

namespace projektaflevering
{
    public partial class TilfoejFlowWindow : Window
    {
        public Flow NytFlow { get; private set; }

        public TilfoejFlowWindow()
        {
            InitializeComponent();
        }

        private void TilfoejKnap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitelBox.Text))
            {
                FejlTekst.Text = "Skriv venligst en titel.";
                return;
            }

            bool synlig = SynligCheckBox.IsChecked == true;

            NytFlow = new Flow(TitelBox.Text.Trim(), BeskrivelseBox.Text.Trim(), synlig);
            DialogResult = true;
            Close();
        }
    }
}

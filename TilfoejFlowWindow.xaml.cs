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

            NytFlow = new Flow(TitelBox.Text.Trim(), BeskrivelseBox.Text.Trim());
            DialogResult = true;
            Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

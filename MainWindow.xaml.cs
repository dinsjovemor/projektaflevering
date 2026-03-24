using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace projektaflevering
{
    public partial class MainWindow : Window
    {
        // Ugens dage
        readonly string[] _dage = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };

        // Skema-indstillinger
        int _startKlokken = 8;
        int _slutKlokken = 20;
        double _pixelPrTime = 60.0;
        double _tidskolonneBredte = 50.0;

        // Samlet højde på skemaet i pixels
        double SamletHøjde => (_slutKlokken - _startKlokken) * _pixelPrTime;

        // Liste over begivenheder i skemaet
        readonly List<SkemaBlok> _begivenheder = new List<SkemaBlok>
        {
            new SkemaBlok(0,  9, 20, 12, 50, "Software arkitektur"),
            new SkemaBlok(0, 14,  0, 15, 30, "Selvstudie"),
            new SkemaBlok(1,  9, 10, 11, 15, "Databaser"),
            new SkemaBlok(2, 11, 30, 12, 30, "Projekt"),
            new SkemaBlok(3,  9,  0, 10, 45, "Projekt"),
            new SkemaBlok(4, 15, 15, 16,  0, "Projekt"),
        };

        // Liste over flows (undervisningsemner)
        readonly List<Flow> _flows = new List<Flow>
        {
            new Flow("Introduktion til C#", "Grundlæggende syntaks, variabler og løkker i C#."),
            new Flow("Objektorienteret programmering", "Klasser, arv og indkapsling."),
            new Flow("WPF og brugergrænseflader", "Sådan bygges vinduer og knapper med XAML."),
        };

        // En canvas pr. dag til at tegne begivenheder på
        readonly Canvas[] _dagCanvasser = new Canvas[7];

        // Den bruger der er logget ind
        readonly Bruger _bruger;

        // Konstruktør – modtager den bruger der loggede ind
        public MainWindow(Bruger bruger)
        {
            InitializeComponent();
            _bruger = bruger;

            // Vis kun knapper hvis brugeren er en underviser
            if (_bruger is Underviser)
            {
                UnderviserPanel.Visibility = Visibility.Visible;
                this.Title = "Ugeskema – Underviser";
            }
            else
            {
                this.Title = "Ugeskema – Studerende";
            }

            // Udfyld flow-listen
            foreach (var flow in _flows)
                FlowListe.Items.Add(flow);

            Loaded += (s, e) => { BygSkema(); PlacerBegivenheder(); };
        }

        // ── Knap: Tilføj begivenhed ──────────────────────────────────────────
        private void TilfoejBegivenhedKnap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TilfoejBegivenhedWindow(_dage);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                _begivenheder.Add(dialog.NyBlok);

                // Genopbyg skemaet
                SkemaGrid.Children.Clear();
                foreach (var canvas in _dagCanvasser)
                    if (canvas != null) canvas.Children.Clear();

                BygSkema();
                PlacerBegivenheder();
            }
        }

        // ── Knap: Tilføj flow ────────────────────────────────────────────────
        private void TilfoejFlowKnap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TilfoejFlowWindow();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                _flows.Add(dialog.NytFlow);
                FlowListe.Items.Add(dialog.NytFlow);
            }
        }

        // ── Valgt flow i listen – vis detaljer ───────────────────────────────
        private void FlowListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlowListe.SelectedItem is Flow valgtFlow)
            {
                FlowTitelTekst.Text = valgtFlow.Titel;
                FlowBeskrivelseTekst.Text = valgtFlow.Beskrivelse;
            }
        }

        // ── Byg skemaoversigten ──────────────────────────────────────────────
        private void BygSkema()
        {
            // Kolonner: tidskolonne + 7 dage
            SkemaGrid.ColumnDefinitions.Clear();
            SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_tidskolonneBredte) });
            for (int d = 0; d < _dage.Length; d++)
                SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Rækker: overskrift + indhold
            SkemaGrid.RowDefinitions.Clear();
            SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(SamletHøjde) });

            // Hjørne (tomt felt øverst til venstre)
            var hjørne = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            Grid.SetRow(hjørne, 0); Grid.SetColumn(hjørne, 0);
            SkemaGrid.Children.Add(hjørne);

            // Dag-overskrifter
            for (int d = 0; d < _dage.Length; d++)
            {
                var overskrift = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 1, 1, 1) };
                overskrift.Child = new TextBlock
                {
                    Text = _dage[d],
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(overskrift, 0); Grid.SetColumn(overskrift, d + 1);
                SkemaGrid.Children.Add(overskrift);
            }

            // Tidskolonne (viser klokkeslæt)
            var tidsCanvas = new Canvas { Width = _tidskolonneBredte, Height = SamletHøjde };
            var tidsBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1, 0, 1, 1),
                Child = tidsCanvas
            };
            Grid.SetRow(tidsBorder, 1); Grid.SetColumn(tidsBorder, 0);
            SkemaGrid.Children.Add(tidsBorder);

            for (int t = _startKlokken; t <= _slutKlokken; t++)
            {
                double y = (t - _startKlokken) * _pixelPrTime;
                var tidslabel = new TextBlock
                {
                    Text = $"{t:D2}:00",
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    Width = _tidskolonneBredte - 4,
                    TextAlignment = TextAlignment.Right
                };
                Canvas.SetTop(tidslabel, y - 7);
                Canvas.SetLeft(tidslabel, 0);
                tidsCanvas.Children.Add(tidslabel);
            }

            // Dag-kolonner
            for (int d = 0; d < _dage.Length; d++)
            {
                var canvas = new Canvas { Background = Brushes.White, Height = SamletHøjde };
                _dagCanvasser[d] = canvas;

                // Vandrette linjer pr. time
                for (int t = _startKlokken; t <= _slutKlokken; t++)
                {
                    double y = (t - _startKlokken) * _pixelPrTime;
                    var linje = new System.Windows.Shapes.Line
                    {
                        X1 = 0, Y1 = y, X2 = 2000, Y2 = y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(linje);
                }

                var dagBorder = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = canvas
                };
                Grid.SetRow(dagBorder, 1); Grid.SetColumn(dagBorder, d + 1);
                SkemaGrid.Children.Add(dagBorder);
            }
        }

        // ── Placer begivenheder på skemaet ───────────────────────────────────
        private void PlacerBegivenheder()
        {
            foreach (var beg in _begivenheder)
            {
                var canvas = _dagCanvasser[beg.DagIndeks];

                double top = TidTilPixel(beg.StartTime, beg.StartMinut);
                double højde = Math.Max(TidTilPixel(beg.SlutTime, beg.SlutMinut) - top, 16);

                var blok = new Border
                {
                    Background = Brushes.LightSteelBlue,
                    BorderBrush = Brushes.SteelBlue,
                    BorderThickness = new Thickness(1),
                    Height = højde,
                    Margin = new Thickness(2, 0, 2, 0),
                    ToolTip = $"{beg.Titel}  {beg.StartTime:D2}:{beg.StartMinut:D2}–{beg.SlutTime:D2}:{beg.SlutMinut:D2}"
                };

                var indhold = new StackPanel { Margin = new Thickness(4, 2, 4, 2) };
                indhold.Children.Add(new TextBlock
                {
                    Text = beg.Titel,
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap
                });
                indhold.Children.Add(new TextBlock
                {
                    Text = $"{beg.StartTime:D2}:{beg.StartMinut:D2}–{beg.SlutTime:D2}:{beg.SlutMinut:D2}",
                    FontSize = 10,
                    Foreground = Brushes.DimGray
                });
                blok.Child = indhold;

                Canvas.SetTop(blok, top);
                canvas.Children.Add(blok);

                // Justér bredden når kolonnen ændrer størrelse
                canvas.SizeChanged += (s, e) =>
                {
                    if (s is Canvas c)
                        foreach (var barn in c.Children)
                            if (barn is Border b)
                                b.Width = c.ActualWidth - 4;
                };
            }
        }

        // Hjælpemetode: omregn klokkeslæt til pixel-position
        private double TidTilPixel(int time, int minut)
            => (time - _startKlokken + minut / 60.0) * _pixelPrTime;
    }
}

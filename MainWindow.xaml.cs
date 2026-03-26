using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace projektaflevering
{
    public partial class MainWindow : Window
    {
        // ── Konstanter ───────────────────────────────────────────────────────
        readonly string[] _dage = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };

        int    _startKlokken       = 8;
        int    _slutKlokken        = 20;
        double _pixelPrTime        = 60.0;
        double _tidskolonneBredte  = 50.0;

        double SamletHøjde => (_slutKlokken - _startKlokken) * _pixelPrTime;

        // ── De tre mønstre ───────────────────────────────────────────────────

        // Observer-mønstret: SkemaLager holder data og sender beskeder
        readonly SkemaLager _lager;

        // Strategy-mønstret: JsonGemmeStrategi håndterer gem/indlæs
        readonly IGemmeStrategy _gemmeStrategi;

        // Den indloggede bruger
        readonly Bruger _bruger;

        // Canvas pr. dag (bruges til at tegne begivenheder)
        readonly Canvas[] _dagCanvasser = new Canvas[7];

        // ── Konstruktør ──────────────────────────────────────────────────────
        public MainWindow(Bruger bruger)
        {
            InitializeComponent();
            _bruger = bruger;

            // Opret lager og gem-strategi
            _lager          = new SkemaLager();
            _gemmeStrategi  = new TekstfilGemmeStrategy("skema.txt");

            // Observer-mønstret: abonner på ændringer i lageret
            // Når data ændres, opdateres brugergrænsefladen automatisk
            _lager.EventListChanged += OpdaterSkema;
            _lager.FlowListChanged += OpdaterFlowListe;

            // Vis underviser-knapper kun for undervisere
            if (_bruger is Underviser)
            {
                UnderviserPanel.Visibility         = Visibility.Visible;
                UnderviserFlowKnapPanel.Visibility = Visibility.Visible;
                this.Title = "Ugeskema – Underviser";
            }
            else
            {
                this.Title = "Ugeskema – Studerende";
            }

            Loaded += Vindue_Loaded;
        }

        // ── Indlæsning ved opstart ───────────────────────────────────────────
        private void Vindue_Loaded(object sender, RoutedEventArgs e)
        {
            // Indlæs gemt data fra JSON-filen (Strategy-mønstret)
            _gemmeStrategi.Indlaes(_lager);

            // Hvis ingen data fandtes, brug standarddata
            if (_lager.Begivenheder.Count == 0)
            {
                _lager.Begivenheder.Add(new SkemaBlok(0,  9, 20, 12, 50, "Software arkitektur", "Introduktion til C#"));
                _lager.Begivenheder.Add(new SkemaBlok(0, 14,  0, 15, 30, "Selvstudie", ""));
                _lager.Begivenheder.Add(new SkemaBlok(1,  9, 10, 11, 15, "Databaser", ""));
                _lager.Begivenheder.Add(new SkemaBlok(2, 11, 30, 12, 30, "Projekt", ""));
                _lager.Begivenheder.Add(new SkemaBlok(3,  9,  0, 10, 45, "Projekt", ""));
                _lager.Begivenheder.Add(new SkemaBlok(4, 15, 15, 16,  0, "Projekt", ""));
            }

            if (_lager.Flows.Count == 0)
            {
                _lager.Flows.Add(new Flow("Introduktion til C#", "Grundlæggende syntaks, variabler og løkker i C#."));
                _lager.Flows.Add(new Flow("Objektorienteret programmering", "Klasser, arv og indkapsling."));
                _lager.Flows.Add(new Flow("WPF og brugergrænseflader", "Sådan bygges vinduer og knapper med XAML."));
            }

            // Byg skemaets grundstruktur (kolonner, rækker, tidslinjer)
            BygSkema();

            // Placer begivenheder og flows (samme som OpdaterSkema/OpdaterFlowListe)
            OpdaterSkema();
            OpdaterFlowListe();
        }

        // ── Observer: opdater skema når data ændres ──────────────────────────
        private void OpdaterSkema()
        {
            // Ryd eksisterende blokke fra canvas'erne
            foreach (var canvas in _dagCanvasser)
                canvas?.Children.Clear();

            // Tilføj timelinjer igen (de fjernes med Children.Clear)
            GenopretTimelinjerne();

            // Placer alle begivenheder
            PlacerBegivenheder();
        }

        // ── Observer: opdater flow-listen når data ændres ────────────────────
        private void OpdaterFlowListe()
        {
            FlowListe.Items.Clear();
            foreach (var flow in _lager.Flows)
            {
                // Studerende ser kun flows markeret som synlige
                if (!(_bruger is Underviser) && !flow.Synlig)
                    continue;
                FlowListe.Items.Add(flow);
            }

            // Ryd detaljevisning
            FlowTitelTekst.Text       = "";
            FlowBeskrivelseTekst.Text = "";
        }

        // ── Hændelse: flow valgt i listen ─────────────────────────────────────
        private void FlowListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlowListe.SelectedItem is Flow valgtFlow)
            {
                FlowTitelTekst.Text       = valgtFlow.Titel;
                FlowBeskrivelseTekst.Text = valgtFlow.Beskrivelse;
            }
        }

        // ── Knap: Tilføj begivenhed ──────────────────────────────────────────
        private void TilfoejBegivenhedKnap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TilfoejBegivenhedWindow(_dage, _lager.Flows);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                // Command-mønstret: brug en kommando til at tilføje
                ICommand kommando = new TilfoejBegivenhedCommand(_lager, dialog.NyBlok);
                kommando.Execute(); // Tilføjer og giver besked til observatører

                _gemmeStrategi.Gem(_lager); // Gem til JSON (Strategy)
            }
        }

        // ── Knap: Tilføj flow ────────────────────────────────────────────────
        private void TilfoejFlowKnap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TilfoejFlowWindow();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                ICommand kommando = new TilfoejFlowCommand(_lager, dialog.NytFlow);
                kommando.Execute();

                _gemmeStrategi.Gem(_lager);
            }
        }

        // ── Knap: Rediger valgt flow ─────────────────────────────────────────
        private void RedigerFlowKnap_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem is Flow valgtFlow)
            {
                var dialog = new RedigerFlowWindow(valgtFlow);
                dialog.Owner = this;

                if (dialog.ShowDialog() == true)
                {
                    ICommand kommando = new RedigerFlowCommand(_lager, valgtFlow, dialog.NyTitel, dialog.NyBeskrivelse);
                    kommando.Execute();

                    _gemmeStrategi.Gem(_lager);
                }
            }
            else
            {
                MessageBox.Show("Vælg venligst et flow i listen.", "Intet valgt");
            }
        }

        // ── Knap: Slet valgt flow ────────────────────────────────────────────
        private void SletFlowKnap_Click(object sender, RoutedEventArgs e)
        {
            if (FlowListe.SelectedItem is Flow valgtFlow)
            {
                var svar = MessageBox.Show(
                    $"Er du sikker på du vil slette \"{valgtFlow.Titel}\"?",
                    "Bekræft sletning",
                    MessageBoxButton.YesNo);

                if (svar == MessageBoxResult.Yes)
                {
                    ICommand kommando = new SletFlowCommand(_lager, valgtFlow);
                    kommando.Execute();

                    _gemmeStrategi.Gem(_lager);
                }
            }
            else
            {
                MessageBox.Show("Vælg venligst et flow i listen.", "Intet valgt");
            }
        }

        // ── Rediger begivenhed (kaldes fra højreklikmenu) ────────────────────
        private void RedigerBegivenhed(SkemaBlok blok)
        {
            var dialog = new RedigerBegivenhedWindow(blok, _dage, _lager.Flows);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                ICommand kommando = new RedigerBegivenhedCommand(_lager, blok, dialog.NyBlok);
                kommando.Execute();

                _gemmeStrategi.Gem(_lager);
            }
        }

        // ── Slet begivenhed (kaldes fra højreklikmenu) ───────────────────────
        private void SletBegivenhed(SkemaBlok blok)
        {
            var svar = MessageBox.Show(
                $"Er du sikker på du vil slette \"{blok.Titel}\"?",
                "Bekræft sletning",
                MessageBoxButton.YesNo);

            if (svar == MessageBoxResult.Yes)
            {
                ICommand kommando = new SletBegivenhedCommand(_lager, blok);
                kommando.Execute();

                _gemmeStrategi.Gem(_lager);
            }
        }

        // ── Vis flow tilknyttet en skemablok ─────────────────────────────────
        private void VisFlowForBlok(SkemaBlok blok)
        {
            if (string.IsNullOrEmpty(blok.FlowTitel))
            {
                MessageBox.Show("Denne begivenhed er ikke tilknyttet et flow.", "Ingen tilknytning");
                return;
            }

            // Find det matchende flow i listen
            foreach (var item in FlowListe.Items)
            {
                if (item is Flow flow && flow.Titel == blok.FlowTitel)
                {
                    // Skift til Flows-fanebladet (indeks 1) og vælg flowet
                    HovedTab.SelectedIndex = 1;
                    FlowListe.SelectedItem = flow;
                    FlowListe.ScrollIntoView(flow);
                    return;
                }
            }

            MessageBox.Show($"Flowet \"{blok.FlowTitel}\" blev ikke fundet.", "Flow ikke fundet");
        }

        // ── Byg skemaets grundstruktur (kolonner, rækker, tidskolonne) ───────
        private void BygSkema()
        {
            SkemaGrid.Children.Clear();
            SkemaGrid.ColumnDefinitions.Clear();
            SkemaGrid.RowDefinitions.Clear();

            // Kolonner: tidskolonne + 7 dag-kolonner
            SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_tidskolonneBredte) });
            for (int d = 0; d < _dage.Length; d++)
                SkemaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Rækker: overskrift + indhold
            SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            SkemaGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(SamletHøjde) });

            // Hjørnefelt (tomt)
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
                    VerticalAlignment   = VerticalAlignment.Center
                };
                Grid.SetRow(overskrift, 0); Grid.SetColumn(overskrift, d + 1);
                SkemaGrid.Children.Add(overskrift);
            }

            // Tidskolonne med klokkeslæt
            var tidsCanvas = new Canvas { Width = _tidskolonneBredte, Height = SamletHøjde };
            var tidsBorder = new Border
            {
                BorderBrush     = Brushes.Gray,
                BorderThickness = new Thickness(1, 0, 1, 1),
                Child           = tidsCanvas
            };
            Grid.SetRow(tidsBorder, 1); Grid.SetColumn(tidsBorder, 0);
            SkemaGrid.Children.Add(tidsBorder);

            for (int t = _startKlokken; t <= _slutKlokken; t++)
            {
                double y = (t - _startKlokken) * _pixelPrTime;
                var tidslabel = new TextBlock
                {
                    Text          = $"{t:D2}:00",
                    FontSize      = 11,
                    Foreground    = Brushes.Gray,
                    Width         = _tidskolonneBredte - 4,
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

                var dagBorder = new Border
                {
                    BorderBrush     = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child           = canvas
                };
                Grid.SetRow(dagBorder, 1); Grid.SetColumn(dagBorder, d + 1);
                SkemaGrid.Children.Add(dagBorder);
            }

            GenopretTimelinjerne();
        }

        // ── Tegn vandrette timelinjer i alle dag-kolonner ────────────────────
        private void GenopretTimelinjerne()
        {
            for (int d = 0; d < _dagCanvasser.Length; d++)
            {
                var canvas = _dagCanvasser[d];
                if (canvas == null) continue;

                for (int t = _startKlokken; t <= _slutKlokken; t++)
                {
                    double y = (t - _startKlokken) * _pixelPrTime;
                    var linje = new System.Windows.Shapes.Line
                    {
                        X1 = 0, Y1 = y, X2 = 2000, Y2 = y,
                        Stroke          = Brushes.LightGray,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(linje);
                }
            }
        }

        // ── Placer begivenheder på skemaet ───────────────────────────────────
        private void PlacerBegivenheder()
        {
            foreach (var beg in _lager.Begivenheder)
            {
                // Studerende ser kun begivenheder markeret som synlige
                if (!(_bruger is Underviser) && !beg.Synlig)
                    continue;

                // Sikkerhed: spring over hvis dagindeks er ugyldigt
                if (beg.DagIndeks < 0 || beg.DagIndeks >= _dagCanvasser.Length) continue;

                var canvas = _dagCanvasser[beg.DagIndeks];
                if (canvas == null) continue;

                double top   = TidTilPixel(beg.StartTime, beg.StartMinut);
                double højde = Math.Max(TidTilPixel(beg.SlutTime, beg.SlutMinut) - top, 16);

                // Baggrundsfarve: blå hvis den har et tilknyttet flow, ellers lysegrøn
                Brush baggrund    = string.IsNullOrEmpty(beg.FlowTitel)
                    ? Brushes.LightSteelBlue
                    : Brushes.LightGreen;
                Brush kantfarve   = string.IsNullOrEmpty(beg.FlowTitel)
                    ? Brushes.SteelBlue
                    : Brushes.Green;

                var blok = new Border
                {
                    Background      = baggrund,
                    BorderBrush     = kantfarve,
                    BorderThickness = new Thickness(1),
                    Height          = højde,
                    Margin          = new Thickness(2, 0, 2, 0),
                    Cursor          = Cursors.Hand,
                    ToolTip         = string.IsNullOrEmpty(beg.FlowTitel)
                        ? beg.Titel
                        : $"{beg.Titel} (Flow: {beg.FlowTitel})"
                };

                var indhold = new StackPanel { Margin = new Thickness(4, 2, 4, 2) };
                indhold.Children.Add(new TextBlock
                {
                    Text        = beg.Titel,
                    FontSize    = 11,
                    FontWeight  = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap
                });
                indhold.Children.Add(new TextBlock
                {
                    Text       = $"{beg.StartTime:D2}:{beg.StartMinut:D2}–{beg.SlutTime:D2}:{beg.SlutMinut:D2}",
                    FontSize   = 10,
                    Foreground = Brushes.DimGray
                });

                // Vis flow-navn hvis tilknyttet
                if (!string.IsNullOrEmpty(beg.FlowTitel))
                {
                    indhold.Children.Add(new TextBlock
                    {
                        Text       = $"→ {beg.FlowTitel}",
                        FontSize   = 9,
                        Foreground = Brushes.DarkGreen,
                        TextWrapping = TextWrapping.Wrap
                    });
                }

                blok.Child = indhold;
                Canvas.SetTop(blok, top);
                canvas.Children.Add(blok);

                // Gem reference til beg så hændelser kan bruge den
                var begRef = beg;

                // Venstreklik: vis tilknyttet flow
                blok.MouseLeftButtonUp += (s, e) =>
                {
                    VisFlowForBlok(begRef);
                    e.Handled = true;
                };

                // Højreklik-menu for undervisere
                if (_bruger is Underviser)
                {
                    var menu = new ContextMenu();

                    var redigerItem = new MenuItem { Header = "Rediger" };
                    redigerItem.Click += (s, e) => RedigerBegivenhed(begRef);

                    var sletItem = new MenuItem { Header = "Slet" };
                    sletItem.Click += (s, e) => SletBegivenhed(begRef);

                    menu.Items.Add(redigerItem);
                    menu.Items.Add(sletItem);
                    blok.ContextMenu = menu;
                }

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

        // ── Hjælpemetode: klokkeslæt → pixel-position ───────────────────────
        private double TidTilPixel(int time, int minut)
            => (time - _startKlokken + minut / 60.0) * _pixelPrTime;
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace projektaflevering
{
    public partial class MainWindow : Window
    {
        readonly string[] _dage = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };
        int StartTime = 8;
        int SlutTime = 20;
        double TimePixelH = 60.0;
        double TidKolonneB = 50.0;

        double TotalH => (SlutTime - StartTime) * TimePixelH;



        readonly List<SkemaBlok> _events = new List<SkemaBlok>
        {
            new SkemaBlok(0,  9, 20, 12, 50, "Software arkitektur"),
            new SkemaBlok(0, 14,  0, 15, 30, "Selvstudie"),
            new SkemaBlok(1,  9, 10, 11, 15, "Databaser"),
            new SkemaBlok(2, 11, 30, 12, 30, "Projekt"),
            new SkemaBlok(3,  9,  0, 10, 45, "Projekt"),
            new SkemaBlok(4, 15, 15, 16,  0, "Projekt"),
        };

        readonly Canvas[] _dagCanvases = new Canvas[7];

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => { BuildLayout(); PlaceEvents(); };
        }

        private void BuildLayout()
        {
            // Column definitions: time label + 7 day columns
            BodyGrid.ColumnDefinitions.Clear();
            BodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(TidKolonneB) });
            for (int d = 0; d < _dage.Length; d++)
            {
                BodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Row definitions: header + body
            BodyGrid.RowDefinitions.Clear();
            BodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });  // header
            BodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(TotalH) }); // body

            // ── Header row ────────────────────────────────────────────────────
            // Empty corner
            var corner = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            Grid.SetRow(corner, 0); Grid.SetColumn(corner, 0);
            BodyGrid.Children.Add(corner);

            for (int d = 0; d < _dage.Length; d++)
            {
                var hdr = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 1, 1, 1)
                };
                hdr.Child = new TextBlock
                {
                    Text = _dage[d],
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(hdr, 0); Grid.SetColumn(hdr, d + 1);
                BodyGrid.Children.Add(hdr);
            }

            // ── Body row ──────────────────────────────────────────────────────

            // Time label canvas
            var timeCanvas = new Canvas { Width = TidKolonneB, Height = TotalH };
            var timeBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1, 0, 1, 1),
                Child = timeCanvas
            };
            Grid.SetRow(timeBorder, 1); Grid.SetColumn(timeBorder, 0);
            BodyGrid.Children.Add(timeBorder);

            for (int h = StartTime; h <= SlutTime; h++)
            {
                double y = (h - StartTime) * TimePixelH;
                var lbl = new TextBlock
                {
                    Text = $"{h:D2}:00",
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
                lbl.Width = TidKolonneB - 4;
                lbl.TextAlignment = TextAlignment.Right;
                Canvas.SetTop(lbl, y - 7);
                Canvas.SetLeft(lbl, 0);
                timeCanvas.Children.Add(lbl);
            }

            // Day columns
            for (int d = 0; d < _dage.Length; d++)
            {
                var canvas = new Canvas { Background = Brushes.White, Height = TotalH };
                _dagCanvases[d] = canvas;

                // Draw horizontal hour lines
                for (int h = StartTime; h <= SlutTime; h++)
                {
                    double y = (h - StartTime) * TimePixelH;
                    var line = new System.Windows.Shapes.Line
                    {
                        X1 = 0,
                        Y1 = y,
                        X2 = 2000,
                        Y2 = y,   // wide enough; clipped by border
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1
                    };
                    canvas.Children.Add(line);
                }

                var colBorder = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = canvas
                };
                Grid.SetRow(colBorder, 1); Grid.SetColumn(colBorder, d + 1);
                BodyGrid.Children.Add(colBorder);
            }
        }

        // ── Place event blocks on their canvases ──────────────────────────────
        private void PlaceEvents()
        {
            foreach (var ev in _events)
            {
                var canvas = _dagCanvases[ev.DayIndex];

                double top = TimeToPixel(ev.StartHour, ev.StartMinute);
                double height = Math.Max(TimeToPixel(ev.EndHour, ev.EndMinute) - top, 16);

                var block = new Border
                {
                    Background = Brushes.LightSteelBlue,
                    BorderBrush = Brushes.SteelBlue,
                    BorderThickness = new Thickness(1),
                    Height = height,
                    Margin = new Thickness(2, 0, 2, 0),
                    ToolTip = $"{ev.Title}  {ev.StartHour:D2}:{ev.StartMinute:D2}–{ev.EndHour:D2}:{ev.EndMinute:D2}"
                };

                var text = new StackPanel { Margin = new Thickness(4, 2, 4, 2) };
                text.Children.Add(new TextBlock
                {
                    Text = ev.Title,
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap
                });
                text.Children.Add(new TextBlock
                {
                    Text = $"{ev.StartHour:D2}:{ev.StartMinute:D2}–{ev.EndHour:D2}:{ev.EndMinute:D2}",
                    FontSize = 10,
                    Foreground = Brushes.DimGray
                });
                block.Child = text;

                Canvas.SetTop(block, top);
                canvas.Children.Add(block);

                // Set width once canvas is measured
                canvas.SizeChanged += (s, e) =>
                {
                    if (s is Canvas c)
                        foreach (var child in c.Children)
                            if (child is Border b)
                                b.Width = c.ActualWidth - 4;
                };
            }
        }

        private double TimeToPixel(int hour, int minute)
            => (hour - StartTime + minute / 60.0) * TimePixelH;
    }
}

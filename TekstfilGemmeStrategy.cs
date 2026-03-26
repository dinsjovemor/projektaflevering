using System.Collections.Generic;
using System.IO;

namespace projektaflevering
{
    // Strategy pattern: gemmer og indlæser data med en simpel tekst fil.
    //
    // Fil format:
    //
    // FLOW
    // Introduktion til C#
    // Grundlæggende syntaks og løkker
    // true
    // BEGIVENHED
    // 0;9;0;10;30;Forelæsning;Introduktion til C#;true

    public class TekstfilGemmeStrategy : IGemmeStrategy
    {
        private readonly string _filsti;

        public TekstfilGemmeStrategy(string filsti)
        {
            _filsti = filsti;
        }

        public void Gem(SkemaLager lager)
        {
            var linjer = new List<string>();

            // Hvert flow gemmes som 4 linjer: FLOW, titel, beskrivelse, synlig
            foreach (var flow in lager.Flows)
            {
                linjer.Add("FLOW");
                linjer.Add(flow.Titel ?? "");
                linjer.Add(flow.Beskrivelse ?? "");
                linjer.Add(flow.Synlig ? "true" : "false");
            }

            // Hver begivenhed gemmes som 2 linjer: BEGIVENHED, data med semikolon
            foreach (var beg in lager.Begivenheder)
            {
                linjer.Add("BEGIVENHED");
                linjer.Add(
                    beg.DagIndeks  + ";" +
                    beg.StartTime  + ";" +
                    beg.StartMinut + ";" +
                    beg.SlutTime   + ";" +
                    beg.SlutMinut  + ";" +
                    (beg.Titel     ?? "") + ";" +
                    (beg.FlowTitel ?? "") + ";" +
                    (beg.Synlig ? "true" : "false")
                );
            }

            File.WriteAllLines(_filsti, linjer);
        }

        public void Indlaes(SkemaLager lager)
        {
            if (!File.Exists(_filsti))
                return;

            string[] linjer = File.ReadAllLines(_filsti);

            lager.Flows.Clear();
            lager.Begivenheder.Clear();

            int i = 0;
            while (i < linjer.Length)
            {
                string linje = linjer[i].Trim();

                if (linje == "FLOW")
                {
                    // De næste tre linjer er titel, beskrivelse og synlig
                    if (i + 3 < linjer.Length)
                    {
                        string titel       = linjer[i + 1];
                        string beskrivelse = linjer[i + 2];
                        bool   synlig      = linjer[i + 3].Trim() != "false";

                        lager.Flows.Add(new Flow(titel, beskrivelse, synlig));
                        i += 4;
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (linje == "BEGIVENHED")
                {
                    if (i + 1 < linjer.Length)
                    {
                        string[] dele = linjer[i + 1].Split(';');

                        // Vi forventer 8 dele (med synlig som det sidste)
                        if (dele.Length >= 7)
                        {
                            int.TryParse(dele[0], out int dagIndeks);
                            int.TryParse(dele[1], out int startTime);
                            int.TryParse(dele[2], out int startMinut);
                            int.TryParse(dele[3], out int slutTime);
                            int.TryParse(dele[4], out int slutMinut);
                            string titel     = dele[5];
                            string flowTitel = dele[6];

                            // Synlig er det 8. felt – hvis det mangler, antag true
                            bool synlig = dele.Length < 8 || dele[7].Trim() != "false";

                            lager.Begivenheder.Add(new SkemaBlok(
                                dagIndeks, startTime, startMinut,
                                slutTime, slutMinut, titel, flowTitel, synlig));
                        }

                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    i++;
                }
            }
        }
    }
}

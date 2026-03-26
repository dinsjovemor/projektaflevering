using System.Collections.Generic;

namespace projektaflevering
{
    // Observer pattern: SkemaLager holder al data og giver besked
    // til alle tilmeldte lyttere når noget ændrer sig.
    //
    // I stedet for event/Action bruger vi en simpel liste af lyttere.
    // Lytterne skal implementere ISkemaObserver-interfacet.
    public class SkemaLager
    {
        // De to datalister
        public List<SkemaBlok> Begivenheder { get; } = new List<SkemaBlok>();
        public List<Flow>      Flows        { get; } = new List<Flow>();

        // Liste over alle der lytter efter ændringer (fx MainWindow)
        private List<ISkemaObserver> _lyttere = new List<ISkemaObserver>();

        // Tilmeld en lytter – fx kaldt fra MainWindow's konstruktør
        public void TilmeldLytter(ISkemaObserver lytter)
        {
            _lyttere.Add(lytter);
        }

        // Hjælpemetode: giv besked til alle lyttere om at begivenheder er ændret
        private void GivBeskedException()
        {
            foreach (var lytter in _lyttere)
                lytter.BegivenhedListeAendret();
        }

        // Hjælpemetode: giv besked til alle lyttere om at flows er ændret
        private void GivBeskedFlow()
        {
            foreach (var lytter in _lyttere)
                lytter.FlowListeAendret();
        }

        // ── Begivenheder ──────────────────────────────────────────────────
        public void TilfoejBegivenhed(SkemaBlok blok)
        {
            Begivenheder.Add(blok);
            GivBeskedException(); // Fortæl lytterne at listen er ændret
        }

        public void FjernBegivenhed(SkemaBlok blok)
        {
            Begivenheder.Remove(blok);
            GivBeskedException();
        }

        public void RedigerBegivenhed(SkemaBlok gammel, SkemaBlok ny)
        {
            int indeks = Begivenheder.IndexOf(gammel);
            if (indeks >= 0)
                Begivenheder[indeks] = ny;
            GivBeskedException();
        }

        // ── Flows ─────────────────────────────────────────────────────────
        public void TilfoejFlow(Flow flow)
        {
            Flows.Add(flow);
            GivBeskedFlow(); // Fortæl lytterne at listen er ændret
        }

        public void FjernFlow(Flow flow)
        {
            Flows.Remove(flow);
            GivBeskedFlow();
        }

        public void RedigerFlow(Flow gammel, string nyTitel, string nyBeskrivelse, bool nySynlig)
        {
            gammel.Titel       = nyTitel;
            gammel.Beskrivelse = nyBeskrivelse;
            gammel.Synlig      = nySynlig;
            GivBeskedFlow();
        }
    }
}

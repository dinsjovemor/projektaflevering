using System;
using System.Collections.Generic;

namespace projektaflevering
{
    // Observer pattern: SkemaLager holds all data and notifies observers
    // when something changes via events.
    // MainWindow subscribes to these events and updates the UI automatically.
    public class SkemaLager
    {
        public List<SkemaBlok> Begivenheder { get; } = new List<SkemaBlok>();
        public List<Flow>      Flows        { get; } = new List<Flow>();

        // Events that observers subscribe to
        public event Action EventListChanged;
        public event Action FlowListChanged;

        // ── Events ───────────────────────────────────────────────────────
        public void TilfoejBegivenhed(SkemaBlok blok)
        {
            Begivenheder.Add(blok);
            EventListChanged?.Invoke();
        }

        public void FjernBegivenhed(SkemaBlok blok)
        {
            Begivenheder.Remove(blok);
            EventListChanged?.Invoke();
        }

        public void RedigerBegivenhed(SkemaBlok gammel, SkemaBlok ny)
        {
            int indeks = Begivenheder.IndexOf(gammel);
            if (indeks >= 0)
                Begivenheder[indeks] = ny;
            EventListChanged?.Invoke();
        }

        // ── Flows ─────────────────────────────────────────────────────────
        public void TilfoejFlow(Flow flow)
        {
            Flows.Add(flow);
            FlowListChanged?.Invoke();
        }

        public void FjernFlow(Flow flow)
        {
            Flows.Remove(flow);
            FlowListChanged?.Invoke();
        }

        public void RedigerFlow(Flow gammel, string nyTitel, string nyBeskrivelse)
        {
            gammel.Titel       = nyTitel;
            gammel.Beskrivelse = nyBeskrivelse;
            FlowListChanged?.Invoke();
        }
    }
}

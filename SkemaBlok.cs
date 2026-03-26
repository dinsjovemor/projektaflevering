namespace projektaflevering
{
    public class SkemaBlok
    {
        public int    DagIndeks  { get; set; }
        public int    StartTime  { get; set; }
        public int    StartMinut { get; set; }
        public int    SlutTime   { get; set; }
        public int    SlutMinut  { get; set; }
        public string Titel      { get; set; }
        public string FlowTitel  { get; set; }

        // Bestemmer om studerende kan se denne begivenhed
        public bool Synlig { get; set; }

        public SkemaBlok() { }

        public SkemaBlok(int dagIndeks, int startTime, int startMinut,
                         int slutTime, int slutMinut, string titel,
                         string flowTitel = "", bool synlig = true)
        {
            DagIndeks  = dagIndeks;
            StartTime  = startTime;
            StartMinut = startMinut;
            SlutTime   = slutTime;
            SlutMinut  = slutMinut;
            Titel      = titel;
            FlowTitel  = flowTitel;
            Synlig     = synlig;
        }
    }
}

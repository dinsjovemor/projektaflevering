namespace projektaflevering
{
    public class Flow
    {
        public string Titel       { get; set; }
        public string Beskrivelse { get; set; }

        // Bestemmer om studerende kan se dette flow
        public bool Synlig { get; set; }

        public Flow() { }

        public Flow(string titel, string beskrivelse, bool synlig = true)
        {
            Titel       = titel;
            Beskrivelse = beskrivelse;
            Synlig      = synlig;
        }

        public override string ToString() => Titel;
    }
}

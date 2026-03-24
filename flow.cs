namespace projektaflevering
{
    public class Flow
    {
        public string Titel { get; set; }
        public string Beskrivelse { get; set; }

        public Flow(string titel, string beskrivelse)
        {
            Titel = titel;
            Beskrivelse = beskrivelse;
        }

        // Bruges til at vise titlen i listeboksen
        public override string ToString()
        {
            return Titel;
        }
    }
}

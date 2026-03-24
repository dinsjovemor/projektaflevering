namespace projektaflevering
{
    public class SkemaBlok
    {
        public int DagIndeks { get; }       // 0 = Mandag, 1 = Tirsdag osv.
        public int StartTime { get; }
        public int StartMinut { get; }
        public int SlutTime { get; }
        public int SlutMinut { get; }
        public string Titel { get; }

        public SkemaBlok(int dagIndeks, int startTime, int startMinut, int slutTime, int slutMinut, string titel)
        {
            DagIndeks = dagIndeks;
            StartTime = startTime;
            StartMinut = startMinut;
            SlutTime = slutTime;
            SlutMinut = slutMinut;
            Titel = titel;
        }
    }
}

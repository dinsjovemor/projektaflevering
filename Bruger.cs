namespace projektaflevering
{
    // Basisklasse for alle brugere
    public class Bruger
    {
        public string Brugernavn { get; set; }
        public string Adgangskode { get; set; }

        public Bruger(string brugernavn, string adgangskode)
        {
            Brugernavn = brugernavn;
            Adgangskode = adgangskode;
        }
    }
}

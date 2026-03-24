namespace projektaflevering
{
    // Studerende arver fra Bruger, men kan ikke redigere skemaet
    public class Studerende : Bruger
    {
        public Studerende(string brugernavn, string adgangskode)
            : base(brugernavn, adgangskode)
        {
        }
    }
}

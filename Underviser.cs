namespace projektaflevering
{
    // Underviser arver fra Bruger, og må redigere skemaet og flows
    public class Underviser : Bruger
    {
        public Underviser(string brugernavn, string adgangskode)
            : base(brugernavn, adgangskode)
        {
        }
    }
}

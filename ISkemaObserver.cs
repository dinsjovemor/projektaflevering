namespace projektaflevering
{
    // Observer pattern: dette interface definerer hvad en "lytter" skal kunne.
    // En lytter er en klasse der gerne vil vide når data ændrer sig.
    //
    // Sådan virker det i praksis:
    //   1. MainWindow implementerer dette interface
    //   2. MainWindow tilmelder sig SkemaLager som lytter
    //   3. Når SkemaLager ændrer data, kalder den metoderne herunder
    //   4. MainWindow opdaterer så brugergrænsefladen
    //
    // Det er det samme som event/Action, bare skrevet mere eksplicit
    // så det er nemmere at forstå hvad der sker.
    public interface ISkemaObserver
    {
        // Kaldes når listen af begivenheder ændrer sig
        void BegivenhedListeAendret();

        // Kaldes når listen af flows ændrer sig
        void FlowListeAendret();
    }
}

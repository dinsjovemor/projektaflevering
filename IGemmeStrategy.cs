namespace projektaflevering
{
    // Strategy pattern: interface for alle gem/indlæs strategies
    public interface IGemmeStrategy
    {
        void Gem(SkemaLager lager);
        void Indlaes(SkemaLager lager);
    }
}

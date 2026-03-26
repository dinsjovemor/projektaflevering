namespace projektaflevering
{
    // Command pattern: Tilføj begivenhed
    public class TilfoejBegivenhedCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly SkemaBlok  _blok;

        public TilfoejBegivenhedCommand(SkemaLager lager, SkemaBlok blok)
        {
            _lager = lager;
            _blok  = blok;
        }

        public void Execute() => _lager.TilfoejBegivenhed(_blok);
    }

    // Command pattern: Slet begivenhed
    public class SletBegivenhedCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly SkemaBlok  _blok;

        public SletBegivenhedCommand(SkemaLager lager, SkemaBlok blok)
        {
            _lager = lager;
            _blok  = blok;
        }

        public void Execute() => _lager.FjernBegivenhed(_blok);
    }

    // Command pattern: Rediger begivenhed
    public class RedigerBegivenhedCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly SkemaBlok  _gammel;
        private readonly SkemaBlok  _ny;

        public RedigerBegivenhedCommand(SkemaLager lager, SkemaBlok gammel, SkemaBlok ny)
        {
            _lager  = lager;
            _gammel = gammel;
            _ny     = ny;
        }

        public void Execute() => _lager.RedigerBegivenhed(_gammel, _ny);
    }

    // Command pattern: Tilføj flow
    public class TilfoejFlowCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly Flow       _flow;

        public TilfoejFlowCommand(SkemaLager lager, Flow flow)
        {
            _lager = lager;
            _flow  = flow;
        }

        public void Execute() => _lager.TilfoejFlow(_flow);
    }

    // Command pattern: Slet flow
    public class SletFlowCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly Flow       _flow;

        public SletFlowCommand(SkemaLager lager, Flow flow)
        {
            _lager = lager;
            _flow  = flow;
        }

        public void Execute() => _lager.FjernFlow(_flow);
    }

    // Command pattern: Rediger flow
    public class RedigerFlowCommand : ICommand
    {
        private readonly SkemaLager _lager;
        private readonly Flow       _flow;
        private readonly string     _nyTitel;
        private readonly string     _nyBeskrivelse;

        public RedigerFlowCommand(SkemaLager lager, Flow flow, string nyTitel, string nyBeskrivelse)
        {
            _lager         = lager;
            _flow          = flow;
            _nyTitel       = nyTitel;
            _nyBeskrivelse = nyBeskrivelse;
        }

        public void Execute() => _lager.RedigerFlow(_flow, _nyTitel, _nyBeskrivelse);
    }
}

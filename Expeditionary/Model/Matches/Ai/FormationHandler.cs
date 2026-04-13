using Expeditionary.Model.Formations;
using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Ai
{
    public class FormationHandler : FormationHandlerBase
    {
        public override string Id => $"formation-{Formation.Name}";
        public override int Echelon => Formation.Echelon;
        public MatchFormation Formation { get; }
        public override IEnumerable<DiadHandler> Diads => _diads;

        private readonly List<DiadHandler> _diads;

        private FormationHandler(
            MatchFormation formation, IEnumerable<FormationHandler> children, IEnumerable<DiadHandler> diads)
            : base(children)
        {
            Formation = formation;
            _diads = diads.ToList();
        }

        public static FormationHandler Create(MatchFormation formation)
        {
            return new FormationHandler(
                formation,
                formation.ComponentFormations.Select(Create),
                formation.Diads.Select(
                    x => new DiadHandler(
                        new UnitHandler(x.Role, x.Unit),
                        x.Transport == null ? null : new UnitHandler(FormationRole.Transport, x.Transport))));
        }
    }
}

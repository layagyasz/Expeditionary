using Expeditionary.Model.Formations;

namespace Expeditionary.Ai
{
    public class FormationHandler : FormationHandlerBase
    {
        public override string Id => $"formation-{Formation.Name}";
        public override int Echelon => Formation.Echelon;
        public Formation Formation { get; }
        public override IEnumerable<DiadHandler> Diads => _diads;

        private readonly List<DiadHandler> _diads;

        private FormationHandler(
            Formation formation, IEnumerable<FormationHandler> children, IEnumerable<DiadHandler> diads)
            : base(children)
        {
            Formation = formation;
            _diads = diads.ToList();
        }

        public static FormationHandler Create(Formation formation)
        {
            return new FormationHandler(
                formation,
                formation.ComponentFormations.Select(Create),
                formation.UnitsAndRoles.Select(
                    x => new DiadHandler(
                        new UnitHandler(x.Role, x.Unit), 
                        x.Transport == null ? null : new UnitHandler(FormationRole.Transport, x.Transport))));
        }
    }
}

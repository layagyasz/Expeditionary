using Expeditionary.Model.Formations;

namespace Expeditionary.Ai
{
    public class SimpleFormationHandler : FormationHandlerBase
    {
        public override string Id => $"formation-{Formation.Name}";
        public override int Echelon => Formation.Echelon;
        public Formation Formation { get; }
        public IEnumerable<UnitHandler> Units => _units;

        private readonly List<UnitHandler> _units;

        private SimpleFormationHandler(
            Formation formation, IEnumerable<SimpleFormationHandler> children, IEnumerable<UnitHandler> units)
            : base(children)
        {
            Formation = formation;
            _units = units.ToList();
        }

        public static SimpleFormationHandler Create(Formation formation)
        {
            return new SimpleFormationHandler(
                formation,
                formation.ComponentFormations.Select(Create),
                formation.UnitsAndRoles.Select(x => new UnitHandler(x.Item1, x.Item2)));
        }

        public void Add(UnitHandler handler)
        {
            _units.Add(handler);
        }

        public override IEnumerable<UnitHandler> GetUnitHandlers()
        {
            return _units;
        }
    }
}

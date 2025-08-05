using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public class Formation
    {
        public record class Diad(FormationRole Role, Unit Unit, Unit? Transport)
        {
            public IEnumerable<Unit> GetUnits()
            {
                if (Transport != null)
                {
                    yield return Transport;
                }
                yield return Unit;
            }
        }

        public Player Player { get; }
        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }
        public List<Diad> UnitsAndRoles => _diads;
        public List<Formation> ComponentFormations => _componentFormations;

        private readonly List<Diad> _diads;
        private readonly List<Formation> _componentFormations;

        public Formation(
            Player player,
            string name,
            FormationRole role, 
            int echelon,
            IEnumerable<Diad> diads,
            IEnumerable<Formation> componentFormations)
        {
            Player = player;
            Name = name;
            Role = role;
            Echelon = echelon;
            _diads = diads.ToList();
            _componentFormations = componentFormations.ToList();
        }

        public IEnumerable<Diad> GetDiads()
        {
            return Enumerable.Concat(_diads, ComponentFormations.SelectMany(x => x.GetDiads()));
        }

        public AssetValue GetAliveUnitQuantity()
        {
            return Enumerable.Concat(
                _componentFormations.Select(x => x.GetAliveUnitQuantity()),
                _diads.Select(x => x.Unit.Value))
                .Aggregate(AssetValue.None, (x, y) => x + y);
        }

        public override string ToString()
        {
            return $"[Formation: Name={Name}, Role={Role}, Echelon={Echelon}]";
        }
    }
}

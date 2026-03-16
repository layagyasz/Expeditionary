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
        public IEnumerable<Diad> Diads => _diads;
        public IEnumerable<Formation> Components => _components;

        private readonly List<Diad> _diads;
        private readonly List<Formation> _components;

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
            _components = componentFormations.ToList();
        }
        
        public void AddComponent(Formation formation)
        {
            _components.Add(formation);
        }

        public IEnumerable<Diad> GetDiads()
        {
            return Enumerable.Concat(_diads, Components.SelectMany(diad => diad.GetDiads()));
        }

        public IEnumerable<Unit> GetUnits()
        {
            return Enumerable.Concat(
                _diads.SelectMany(diad => diad.GetUnits()), _components.SelectMany(component => component.GetUnits()));
        }

        public AssetValue GetAliveUnitQuantity()
        {
            return Enumerable.Concat(
                _components.Select(x => x.GetAliveUnitQuantity()),
                _diads.Select(x => x.Unit.Value))
                .Aggregate(AssetValue.None, (x, y) => x + y);
        }

        public override string ToString()
        {
            return $"[Formation: Name={Name}, Role={Role}, Echelon={Echelon}]";
        }
    }
}

using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public class Formation
    {
        public Player Player { get; }
        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }
        public List<(Unit, FormationRole)> UnitsAndRoles => _unitsAndRoles;
        public List<Formation> ComponentFormations => _componentFormations;

        private readonly List<(Unit, FormationRole)> _unitsAndRoles;
        private readonly List<Formation> _componentFormations;

        public Formation(
            Player player,
            string name,
            FormationRole role, 
            int echelon,
            IEnumerable<(Unit, FormationRole)> unitsAndRoles,
            IEnumerable<Formation> componentFormations)
        {
            Player = player;
            Name = name;
            Role = role;
            Echelon = echelon;
            _unitsAndRoles = unitsAndRoles.ToList();
            _componentFormations = componentFormations.ToList();
        }

        public IEnumerable<(Unit, FormationRole)> GetUnitsAndRoles()
        {
            return Enumerable.Concat(_unitsAndRoles, ComponentFormations.SelectMany(x => x.GetUnitsAndRoles()));
        }

        public override string ToString()
        {
            return $"[Formation: Name={Name}, Role={Role}, Echelon={Echelon}]";
        }
    }
}

using Expeditionary.Ai.Assignments;
using Expeditionary.Model.Formations;

namespace Expeditionary.Ai
{
    public class FormationAssignment
    {
        public Formation Formation { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment();
        public IEnumerable<FormationAssignment> Children => _children;
        public IEnumerable<UnitAssignment> Units => _units;

        private readonly List<FormationAssignment> _children;
        private readonly List<UnitAssignment> _units;

        private FormationAssignment(
            Formation formation, IEnumerable<FormationAssignment> children, IEnumerable<UnitAssignment> units)
        {
            Formation = formation;
            _children = children.ToList();
            _units = units.ToList();
        }

        public static FormationAssignment Create(Formation formation)
        {
            return new(
                formation,
                formation.ComponentFormations.Select(Create), 
                formation.UnitsAndRoles.Select(x => new UnitAssignment(x.Item1, x.Item2)));
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            Assignment.Assign(this);
        }
    }
}

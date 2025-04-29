using Cardamom.Logging;
using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;

namespace Expeditionary.Ai
{
    public class FormationAssignment
    {
        private readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationAssignment));

        public Formation Formation { get; }
        public IFormationAssignment Assignment { get; private set; } = new NoFormationAssignment();
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

        public void AssignChildren(Match match, EvaluationCache evaluationCache, Random random)
        {
            s_Logger.With(Formation.Name).Log($"redo assignment");
            Assignment.Assign(this, match, evaluationCache, random);
            foreach (var child in _children)
            {
                child.AssignChildren(match, evaluationCache, random);
            }
        }

        public IEnumerable<UnitAssignment> GetUnitAssignments()
        {
            return Enumerable.Concat(_units, _children.SelectMany(x => x.GetUnitAssignments()));
        }

        public void SetAssignment(IFormationAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Formation.Name).Log($"assigned {assignment}");
        }
    }
}

using Cardamom.Logging;
using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;

namespace Expeditionary.Ai
{
    public class FormationHandler
    {
        private readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationHandler));

        public Formation Formation { get; }
        public IFormationAssignment Assignment { get; private set; } = new NoFormationAssignment();
        public IEnumerable<FormationHandler> Children => _children;
        public IEnumerable<UnitHandler> Units => _units;

        private readonly List<FormationHandler> _children;
        private readonly List<UnitHandler> _units;

        private FormationHandler(
            Formation formation, IEnumerable<FormationHandler> children, IEnumerable<UnitHandler> units)
        {
            Formation = formation;
            _children = children.ToList();
            _units = units.ToList();
        }

        public static FormationHandler Create(Formation formation)
        {
            return new(
                formation,
                formation.ComponentFormations.Select(Create), 
                formation.UnitsAndRoles.Select(x => new UnitHandler(x.Item1, x.Item2)));
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

        public IEnumerable<UnitHandler> GetUnitHandlers()
        {
            return Enumerable.Concat(_units, _children.SelectMany(x => x.GetUnitHandlers()));
        }

        public void SetAssignment(IFormationAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Formation.Name).Log($"assigned {assignment}");
        }
    }
}

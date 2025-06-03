using Cardamom.Logging;
using Expeditionary.Ai.Assignments;
using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public abstract class FormationHandlerBase : IFormationHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationHandlerBase));

        public abstract string Id { get; }
        public abstract int Echelon { get; }
        public IFormationAssignment Assignment { get; private set; } = new NoFormationAssignment();
        public IEnumerable<SimpleFormationHandler> Children => _children;

        private readonly List<SimpleFormationHandler> _children;

        protected FormationHandlerBase(IEnumerable<SimpleFormationHandler> children)
        {
            _children = children.ToList();
        }

        public void Add(SimpleFormationHandler handler)
        {
            _children.Add(handler);
        }

        public IEnumerable<SimpleFormationHandler> GetAllFormationHandlers()
        {
            return Enumerable.Concat(_children, _children.SelectMany(x => x.GetAllFormationHandlers()));
        }

        public IEnumerable<UnitHandler> GetAllUnitHandlers()
        {
            return Enumerable.Concat(GetUnitHandlers(), Children.SelectMany(x => x.GetAllUnitHandlers()));
        }

        public abstract IEnumerable<UnitHandler> GetUnitHandlers();

        public void Reevaluate(Match match, TileEvaluator tileEvaluator)
        {
            s_Logger.With(Id).Log($"reevaluate {Assignment}");
            var assignment = Assignment.Assign(this, match, tileEvaluator);
            DoAssignment(assignment);
            foreach (var child in Children)
            {
                child.Reevaluate(match, tileEvaluator);
            }
        }

        public void SetAssignment(IFormationAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Id).Log($"assigned {assignment}");
        }

        private static void DoAssignment(FormationAssignment assignment)
        {
            foreach ((var f, var a) in  assignment.ChildFormationAssignments)
            {
                f.SetAssignment(a);
            }
            foreach ((var u, var a) in assignment.UnitAssignments)
            {
                u.SetAssignment(a);
            }
        }
    }
}

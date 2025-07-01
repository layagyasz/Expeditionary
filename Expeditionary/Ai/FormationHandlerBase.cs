using Cardamom.Logging;
using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public abstract class FormationHandlerBase : IFormationHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationHandlerBase));

        private static readonly float s_ReassignmentWeight = 0.8f;

        public abstract string Id { get; }
        public abstract int Echelon { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment();
        public IEnumerable<SimpleFormationHandler> Children => _children;

        private readonly List<SimpleFormationHandler> _children;

        private bool _isDirty = true;
        private AssignmentRealization? _assignmentRealization;

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
            var newRealization = Assignment.Assign(this, match, tileEvaluator);
            if (_assignmentRealization == null
                || _isDirty
                || s_ReassignmentWeight * Assignment.EvaluateRealization(newRealization, match)
                    > Assignment.EvaluateRealization(_assignmentRealization, match))
            {
                s_Logger.With(Id).Log($"reevaluated {Assignment}");
                _assignmentRealization = newRealization;
                _isDirty = false;
                DoAssignment(_assignmentRealization);
            }
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            _isDirty = true;
            s_Logger.With(Id).Log($"assigned {assignment}");
        }

        private static void DoAssignment(AssignmentRealization assignment)
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

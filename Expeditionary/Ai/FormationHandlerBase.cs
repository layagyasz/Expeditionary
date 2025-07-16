using Cardamom.Logging;
using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;

namespace Expeditionary.Ai
{
    public abstract class FormationHandlerBase : IAiHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationHandlerBase));

        private static readonly float s_ReassignmentWeight = 0.8f;

        public abstract string Id { get; }
        public abstract int Echelon { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment();
        public IEnumerable<FormationHandler> Children => _children;
        public abstract IEnumerable<DiadHandler> Diads { get; }

        private readonly List<FormationHandler> _children;

        private bool _isDirty = true;
        private AssignmentRealization? _assignmentRealization;

        protected FormationHandlerBase(IEnumerable<FormationHandler> children)
        {
            _children = children.ToList();
        }

        public void Add(FormationHandler handler)
        {
            _children.Add(handler);
        }

        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            var newRealization = Assignment.Assign(this, match, tileEvaluator);
            if (_assignmentRealization == null
                || _isDirty
                || s_ReassignmentWeight * Assignment.EvaluateRealization(newRealization, match)
                    > Assignment.EvaluateRealization(_assignmentRealization, match))
            {
                s_Logger.With(Id).Log($"reevaluated {Assignment}");
                _assignmentRealization = newRealization;
                DoAssignment(_assignmentRealization);
                _isDirty = false;
            }
        }

        public void Setup(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            s_Logger.With(Id).Log($"setup {Assignment}");
            var newRealization = Assignment.Assign(this, match, tileEvaluator);
            _assignmentRealization = newRealization;
            DoAssignment(_assignmentRealization);
            _isDirty = false;
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

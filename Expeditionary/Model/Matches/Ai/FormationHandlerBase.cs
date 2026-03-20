using Cardamom.Logging;
using Expeditionary.Model.Matches.Ai.Assignments;

namespace Expeditionary.Model.Matches.Ai
{
    public abstract class FormationHandlerBase : IAiHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(FormationHandlerBase));

        private static readonly float s_ReassignmentWeight = 0.8f;

        public abstract string Id { get; }
        public abstract int Echelon { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment(default);
        public IEnumerable<FormationHandler> Components => _components;
        public abstract IEnumerable<DiadHandler> Diads { get; }

        private readonly List<FormationHandler> _components;

        private bool _isDirty = true;
        private AssignmentRealization? _assignmentRealization;

        protected FormationHandlerBase(IEnumerable<FormationHandler> children)
        {
            _components = children.ToList();
        }

        public void AddComponent(FormationHandler handler)
        {
            _components.Add(handler);
        }

        public AiHandlerStatus DoTurn(Match match)
        {
            var newRealization = Assignment.Assign(this, match);
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

            var statuses = new List<(IAiHandler, AiHandlerStatus)>();
            foreach (var child in Components)
            {
                statuses.Add((child, child.DoTurn(match)));
            }
            foreach (var diad in Diads)
            {
                statuses.Add((diad, diad.DoTurn(match)));
            }
            return AggregateStatus(statuses);
        }

        public Movement.Hindrance GetMaxHindrance()
        {
            return Components.Select(x => x.GetMaxHindrance()).Concat(Diads.Select(x => x.GetMaxHindrance()))
                .Aggregate(Movement.Min);
        }

        public void Setup(Match match)
        {
            s_Logger.With(Id).Log($"setup {Assignment}");
            var newRealization = Assignment.Assign(this, match);
            _assignmentRealization = newRealization;
            DoAssignment(_assignmentRealization);
            _isDirty = false;

            foreach (var child in Components)
            {
                child.Setup(match);
            }
            foreach (var diad in Diads)
            {
                diad.Setup(match);
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
            foreach ((var f, var a) in assignment.ChildFormationAssignments)
            {
                f.SetAssignment(a);
            }
            foreach ((var u, var a) in assignment.UnitAssignments)
            {
                u.SetAssignment(a);
            }
        }

        private AiHandlerStatus AggregateStatus(IEnumerable<(IAiHandler, AiHandlerStatus)> childStatuses)
        {
            // TODO: Implement
            return AiHandlerStatus.Done;
        }
    }
}

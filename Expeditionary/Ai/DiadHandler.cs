using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class DiadHandler : IAiHandler
    {
        public FormationRole Role => Unit.Role;
        public UnitHandler Unit { get; }
        public UnitHandler? Transport { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment(default);
        public IEnumerable<FormationHandler> Children => Enumerable.Empty<FormationHandler>();
        public IEnumerable<DiadHandler> Diads => Enumerable.Empty<DiadHandler>();
        public string Id => $"diad-{Unit.Id}-{Transport?.Id ?? "NA"}";
        public int Echelon => 1;

        private bool _isDirty = true;

        public DiadHandler(UnitHandler unit, UnitHandler? transport)
        {
            Unit = unit;
            Transport = transport;
        }

        public void Add(FormationHandler handler)
        {
            throw new InvalidOperationException();
        }

        public AiHandlerStatus DoTurn(Match match)
        { 
            if (_isDirty)
            {
                if (Assignment is PointAssignment pointAssignment 
                    && ShouldUseTransport(Transport?.Unit, Unit.Unit, pointAssignment, match.GetEvaluator()))
                {
                    Unit.SetAssignment(new NoAssignment(pointAssignment.Origin));
                    Transport!.SetAssignment(
                        new TransportAssignment(
                            Unit.Unit, pointAssignment.Hex, pointAssignment.Origin, pointAssignment.Facing));
                }
                else
                {
                    Unit.SetAssignment(Assignment);
                    Transport?.SetAssignment(GetShadowAssignment(Transport, Assignment, match));
                } 
                _isDirty = false;
            }

            if (Transport?.DoTurn(match) == AiHandlerStatus.Done && Unit.Assignment != Assignment)
            {
                Unit.SetAssignment(Assignment);
            }
            return Unit.DoTurn(match);
        }

        public Movement.Hindrance GetMaxHindrance()
        {
            return Transport == null || Transport.Unit.IsDestroyed 
                ? Unit.GetMaxHindrance() 
                : Transport.GetMaxHindrance();
        }

        public void Setup(Match match)
        {
            Unit.SetAssignment(Assignment);
            Unit.Setup(match);
            Transport?.SetAssignment(GetShadowAssignment(Transport, Assignment, match));
            Transport?.Setup(match);
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            _isDirty = true;
        }

        private static bool ShouldUseTransport(
            Unit? transport, Unit passenger, PointAssignment assignment, TileEvaluator evaluator)
        {
            if (transport == null || !transport.IsActive() || !passenger.IsActive())
            {
                return false;
            }
            if (!OrderChecker.CanLoad(transport.Type, passenger.Type))
            {
                return false;
            }
            var hindrance = transport.Type.Movement.GetMaxHindrance();
            return evaluator.IsReachable(hindrance, transport.Position!.Value, assignment.Hex)
                && evaluator.IsReachable(hindrance, transport.Position!.Value, passenger.Position!.Value);
        }

        private static IAssignment GetShadowAssignment(UnitHandler unit, IAssignment assignment, Match match)
        {
            if (assignment is PointAssignment pointAssignment)
            {
                return PointAssignment.GetShadowPoint(pointAssignment, unit, match);
            }
            return new NoAssignment(assignment.Origin);
        }
    }
}

using Expeditionary.Ai.Assignments;
using Expeditionary.Model;
using Expeditionary.Model.Formations;

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
                Unit.SetAssignment(Assignment);
                Transport?.SetAssignment(GetShadowAssignment(Transport, Assignment, match));
                _isDirty = false;
            }
            
            Transport?.DoTurn(match);
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

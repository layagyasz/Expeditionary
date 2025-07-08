using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;

namespace Expeditionary.Ai
{
    public class DiadHandler : IAiHandler
    {
        public FormationRole Role => Unit.Role;
        public UnitHandler Unit { get; }
        public UnitHandler? Transport { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment();
        public IEnumerable<FormationHandler> Children => Enumerable.Empty<FormationHandler>();
        public IEnumerable<DiadHandler> Diads => Enumerable.Empty<DiadHandler>();
        public string Id => $"diad-{Unit.Id}-{Transport?.Id ?? "NA"}";
        public int Echelon => 1;

        public DiadHandler(UnitHandler unit, UnitHandler? transport)
        {
            Unit = unit;
            Transport = transport;
        }

        public void Add(FormationHandler handler)
        {
            throw new InvalidOperationException();
        }

        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            if (Unit.Unit.Position == null && !Unit.Unit.IsDestroyed)
            {
                Unit.Assignment.Place(Unit, match);
            }
            if (Transport != null && Transport.Unit.Position == null && !Transport.Unit.IsDestroyed)
            {
                Transport.Assignment.Place(Transport, match);
            }
            else
            {
                Unit.DoTurn(match, knowledge, tileEvaluator);
                Transport?.DoTurn(match, knowledge, tileEvaluator);
            }
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            Unit.SetAssignment(Assignment);
            // Transport?.SetAssignment(Assignment);
        }
    }
}

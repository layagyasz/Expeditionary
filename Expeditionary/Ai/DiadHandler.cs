using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping.Regions;

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

        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        { 
            if (_isDirty)
            {
                Unit.SetAssignment(Assignment);
                Transport?.SetAssignment(GetShadowAssignment(Transport, Assignment, match, tileEvaluator));
                _isDirty = false;
            }
            
            Unit.DoTurn(match, knowledge, tileEvaluator);
            Transport?.DoTurn(match, knowledge, tileEvaluator);
        }

        public void Setup(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            Unit.SetAssignment(Assignment);
            Unit.Setup(match, knowledge, tileEvaluator);
            Transport?.SetAssignment(GetShadowAssignment(Transport, Assignment, match, tileEvaluator));
            Transport?.Setup(match, knowledge, tileEvaluator);
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            _isDirty = true;
        }

        private static IAssignment GetShadowAssignment(
            UnitHandler unit, IAssignment assignment, Match match, TileEvaluator tileEvaluator)
        {
            if (assignment is PointAssignment pointAssignment)
            {
                return PointAssignment.GetShadowPoint(pointAssignment, unit, match, tileEvaluator);
            }
            return new NoAssignment();
        }
    }
}

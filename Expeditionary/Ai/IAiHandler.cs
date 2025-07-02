using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;

namespace Expeditionary.Ai
{
    public interface IAiHandler
    {
        public IAssignment Assignment { get; }
        public IEnumerable<FormationHandler> Children { get; }
        public IEnumerable<DiadHandler> Diads { get; }
        public string Id { get; }
        public int Echelon { get; }
        public void Add(FormationHandler handler);
        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator);
        public void SetAssignment(IAssignment assignment);
    }
}

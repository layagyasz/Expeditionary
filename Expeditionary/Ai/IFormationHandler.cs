using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public interface IFormationHandler
    {
        public IAssignment Assignment { get; }
        public IEnumerable<SimpleFormationHandler> Children { get; }
        public string Id { get; }
        public int Echelon { get; }
        public void Add(SimpleFormationHandler handler);
        public void Reevaluate(Match match, TileEvaluator tileEvaluator);
        public IEnumerable<SimpleFormationHandler> GetAllFormationHandlers();
        public IEnumerable<UnitHandler> GetAllUnitHandlers();
        public IEnumerable<UnitHandler> GetUnitHandlers();
        public void SetAssignment(IAssignment assignment);
    }
}

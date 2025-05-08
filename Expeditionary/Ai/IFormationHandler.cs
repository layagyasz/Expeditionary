using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public interface IFormationHandler
    {
        public IFormationAssignment Assignment { get; }
        public IEnumerable<SimpleFormationHandler> Children { get; }
        public string Id { get; }
        public void Add(SimpleFormationHandler handler);
        public void Reevaluate(Match match, EvaluationCache evaluationCache, Random random);
        public IEnumerable<SimpleFormationHandler> GetAllFormationHandlers();
        public IEnumerable<UnitHandler> GetAllUnitHandlers();
        public IEnumerable<UnitHandler> GetUnitHandlers();
        public void SetAssignment(IFormationAssignment assignment);
    }
}

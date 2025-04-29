using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai.Assignments.Formations
{
    public interface IFormationAssignment
    {
        public void Assign(FormationAssignment formation, Match match, EvaluationCache evaluationCache, Random random);
    }
}

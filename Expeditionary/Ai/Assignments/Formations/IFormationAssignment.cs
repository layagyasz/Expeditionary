using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai.Assignments.Formations
{
    public interface IFormationAssignment
    {
        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random);
    }
}

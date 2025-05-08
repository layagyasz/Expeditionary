using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai.Assignments.Formations
{
    public class NoFormationAssignment : IFormationAssignment
    {
        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random) 
        {
            return new(
                formation.Children.ToDictionary(x => x, x => (IFormationAssignment)new NoFormationAssignment()),
                formation.GetUnitHandlers().ToDictionary(x => x, x => (IUnitAssignment)new NoUnitAssignment()));
        }
    }
}

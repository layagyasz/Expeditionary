using Expeditionary.Evaluation;

namespace Expeditionary.Model.Missions
{
    public record PlayerSetupContext(SetupContext Parent, EvaluationCache EvaluationCache);
}

using Expeditionary.Evaluation;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(
        Player Player, Random Random, IIdGenerator IdGenerator, bool IsTest);
}

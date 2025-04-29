using Expeditionary.Ai;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(Random Random, IIdGenerator IdGenerator, AiManager AiManager);
}

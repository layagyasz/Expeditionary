using Expeditionary.Ai;
using Expeditionary.Loader;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(Random Random, IIdGenerator IdGenerator, AiManager AiManager);
}

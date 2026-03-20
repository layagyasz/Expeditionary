using Expeditionary.Loader;
using Expeditionary.Model.Matches.Ai;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(Random Random, IIdGenerator IdGenerator, AiManager AiManager);
}

using Expeditionary.Ai;
using Expeditionary.Loader;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(LoaderStatus Status, Random Random, IIdGenerator IdGenerator, AiManager AiManager);
}

using Expeditionary.Model.Matches.Ai;
using System.Collections.Immutable;

namespace Expeditionary.Model.Missions
{
    public record SetupContext(AiManager AiManager, ImmutableList<PlayerSetupContext> PlayerContexts)
    {
        public PlayerSetupContext GetPlayerContext(Player player)
        {
            return PlayerContexts.Find(context => context.Player == player)!;
        }
    }
}

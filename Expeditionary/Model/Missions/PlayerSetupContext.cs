using Expeditionary.Model.Matches;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetupContext(MatchPlayer Player, bool IsHuman, IFormationProvider FormationProvider);
}

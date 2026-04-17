using Expeditionary.Model.Matches;

namespace Expeditionary.Model.Missions
{
    public record class CreationContext(MatchPlayer Player, Random Random, bool IsTest);
}

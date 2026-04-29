using Expeditionary.Model.Missions.Generator;
using System.Collections.Immutable;

namespace Expeditionary.Model.Campaigns
{
    public record class CampaignNode(int Id, ImmutableList<MissionNode> MissionNodes);
}

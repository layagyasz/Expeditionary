using Expeditionary.Model.Instances.Campaigns;
using Expeditionary.Model.Missions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Instances
{
    public record class InstanceMission(
        CampaignStageKey CampaignStageKey, long StartTime, long EndTime, Vector2 Position, Mission Mission);
}

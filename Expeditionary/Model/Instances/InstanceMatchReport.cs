using Expeditionary.Model.Instances.Campaigns;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Reporting;

namespace Expeditionary.Model.Instances
{
    public record class InstanceMatchReport(CampaignStageKey CampaignStageKey, MatchPlayer Player, MatchReport Report)
    {
        public PlayerReport GetPlayerReport()
        {
            return Report.Players[Player];
        }
    }
}

using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Reporting;

namespace Expeditionary.Model.Instances
{
    public record class InstanceMatchReport(
        string CampaignKey, int CampaignStageId, MatchPlayer Player, MatchReport Report)
    {
        public PlayerReport GetPlayerReport()
        {
            return Report.Players[Player];
        }
    }
}

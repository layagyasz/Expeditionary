using Expeditionary.Model.Instances.Campaigns;
using Expeditionary.Model.Instances.Events;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Reporting;

namespace Expeditionary.Model.Instances
{
    public record class InstanceMatchReport(CampaignStageKey CampaignStageKey, MatchPlayer Player, MatchReport Report)
    {
        public PlayerReport GetPlayerReport()
        {
            return Report.Players[Player];
        }

        public IEnumerable<IInstanceEvent> GetEvents()
        {
            foreach (var unit in Report.Players.Values.SelectMany(player => player.Formation.GetUnits()))
            {
                if (unit.InstanceId < 0)
                {
                    continue;
                }
                yield return new UpdateUnitStatusEvent(unit.InstanceId, ToInstanceEnum(unit.Status), unit.Number);
            }
        }

        private static InstanceUnitStatus ToInstanceEnum(MatchAssetStatus status) => status switch
        {
            MatchAssetStatus.Destroyed => InstanceUnitStatus.Destroyed,
            _ => InstanceUnitStatus.Active
        };
    }
}

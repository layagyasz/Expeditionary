using Expeditionary.Model.Missions.Objectives;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Instances.Campaigns
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(MissionOutcomeTrigger), "MissionOutcome")]
    public interface ICampaignTrigger
    {
        bool Trigger(Campaign campaign, CampaignStage stage, GameInstance instance);

        public record class MissionOutcomeTrigger(
            ObjectiveStatus ObjectiveStatus, 
            CampaignTriggerPredicate ObjectiveStatusPredicate, 
            CampaignTriggerScope Scope,
            int Count)
        {
            public bool Trigger(Campaign campaign, CampaignStage stage, GameInstance instance)
            {
                return instance.MatchHistory
                    .Where(report => IsInScope(report, campaign.Key, stage.Id, Scope))
                    .Count(report => 
                        Evaluate(
                            ObjectiveStatusPredicate, 
                            (int)ObjectiveStatus, 
                            (int)report.GetPlayerReport().ObjectiveStatus))
                    >= Count;
            }
        }

        private static bool Evaluate<T>(CampaignTriggerPredicate predicate, T expected, T actual) 
            where T : IComparable<T>
        {
            var result = actual.CompareTo(expected);
            return predicate switch
            {
                CampaignTriggerPredicate.Equal => result == 0,
                CampaignTriggerPredicate.LessThanOrEqual => result <= 0,
                CampaignTriggerPredicate.GreaterThanOrEqual => result >= 0,
                _ => throw new ArgumentException($"Unsupported CampaignTriggerPredicate {predicate}")
            };
        }

        private static bool IsInScope(
            InstanceMatchReport report, string campaignKey, int stageId, CampaignTriggerScope scope) => scope switch
            {
                CampaignTriggerScope.All => true,
                CampaignTriggerScope.Campaign => report.CampaignKey == campaignKey,
                CampaignTriggerScope.CampaignStage =>
                    report.CampaignKey == campaignKey && report.CampaignStageId == stageId,
                _ => throw new ArgumentException($"Unsupported CampaignTriggerScope {scope}")
            };
    }
}

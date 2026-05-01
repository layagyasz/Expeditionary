using Expeditionary.Model.Missions.Objectives;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Instances.Campaigns
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(AlwaysTrigger), "Always")]
    [JsonDerivedType(typeof(MissionOutcomeTrigger), "MissionOutcome")]
    [JsonDerivedType(typeof(NeverTrigger), "Never")]
    public interface ICampaignTrigger
    {
        bool Trigger(CampaignStageKey stageKey, GameInstance instance);

        public record class AlwaysTrigger() : ICampaignTrigger
        {
            public bool Trigger(CampaignStageKey stageKey, GameInstance instance)
            {
                return true;
            }
        }

        public record class MissionOutcomeTrigger(
            ObjectiveStatus ObjectiveStatus, 
            CampaignTriggerPredicate ObjectiveStatusPredicate, 
            CampaignTriggerScope Scope,
            int Count) : ICampaignTrigger
        {
            public bool Trigger(CampaignStageKey stageKey, GameInstance instance)
            {
                return instance.MatchHistory
                    .Where(report => IsInScope(report, stageKey, Scope))
                    .Count(report => 
                        Evaluate(
                            ObjectiveStatusPredicate, 
                            (int)ObjectiveStatus, 
                            (int)report.GetPlayerReport().ObjectiveStatus))
                    >= Count;
            }
        }

        public record class NeverTrigger() : ICampaignTrigger
        {
            public bool Trigger(CampaignStageKey stageKey, GameInstance instance)
            {
                return false;
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
            InstanceMatchReport report, CampaignStageKey stageKey, CampaignTriggerScope scope) => scope switch
            {
                CampaignTriggerScope.All => true,
                CampaignTriggerScope.Campaign => report.CampaignStageKey.CampaignEquals(stageKey),
                CampaignTriggerScope.CampaignStage => report.CampaignStageKey.Equals(stageKey),
                _ => throw new ArgumentException($"Unsupported CampaignTriggerScope {scope}")
            };
    }
}

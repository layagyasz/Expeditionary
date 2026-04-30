using System.Text.Json.Serialization;

namespace Expeditionary.Model.Instances.Campaigns
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CampaignTriggerScope
    {
        All,
        Campaign,
        CampaignStage
    }
}

using Cardamom.Json;
using Expeditionary.Model.Mapping.Environments;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public record class StaticEnvironmentProvider : IEnvironmentProvider
    {
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public MapEnvironmentDefinition Environment { get; set; } = new();

        public MapEnvironmentDefinition Get(MissionGenerationResources resources)
        {
            return Environment;
        }
    }
}

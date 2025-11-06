using Cardamom.Json;
using Expeditionary.Model.Mapping.Environments;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.Generator
{
    public record class StaticEnvironmentProvider : IEnvironmentProvider
    {
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public required MapEnvironmentDefinition Environment { get; set; }

        public MapEnvironmentDefinition Get(MissionGenerationResources resources)
        {
            return Environment;
        }
    }
}

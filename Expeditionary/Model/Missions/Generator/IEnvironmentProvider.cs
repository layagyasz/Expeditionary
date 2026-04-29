using Expeditionary.Model.Mapping.Environments;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.Generator
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(RandomEnvironmentProvider), "Random")]
    [JsonDerivedType(typeof(StaticEnvironmentProvider), "Static")]
    public interface IEnvironmentProvider
    {
        MapEnvironmentDefinition Get(MissionGenerationResources resources);
    }
}

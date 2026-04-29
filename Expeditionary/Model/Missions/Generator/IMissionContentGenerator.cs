using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.Generator
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(AssaultMissionGenerator), "Assault")]
    public interface IMissionContentGenerator
    {
        MissionContent Generate(MissionNode node, MissionGenerationResources resources);
    }
}

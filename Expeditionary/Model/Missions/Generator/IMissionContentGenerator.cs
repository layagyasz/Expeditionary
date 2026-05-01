using System.Text.Json.Serialization;
using Expeditionary.Model.Instances;

namespace Expeditionary.Model.Missions.Generator
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(AssaultMissionGenerator), "Assault")]
    public interface IMissionContentGenerator
    {
        Mission Generate(MissionNode node, MissionGenerationResources resources);
    }
}

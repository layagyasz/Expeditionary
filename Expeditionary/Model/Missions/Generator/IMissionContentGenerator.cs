namespace Expeditionary.Model.Missions.Generator
{
    public interface IMissionContentGenerator
    {
        MissionContent Generate(MissionNode node, MissionGenerationResources resources);
    }
}

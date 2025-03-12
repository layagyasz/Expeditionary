namespace Expeditionary.Model.Missions.MissionTypes
{
    public interface IMissionNode
    {
        Mission Create(MissionGenerationResources resources);
    }
}

namespace Expeditionary.Model.Missions.MissionTypes
{
    public interface IMissionType
    {
        Mission Create(MissionParameters parameters, MissionGenerationResources resources);
    }
}

namespace Expeditionary.Model.Missions.Objectives
{
    public record struct ObjectiveCompletion(
        ObjectiveStatus Status, ObjectiveDisposition Disposition, bool IsTerminal);
}

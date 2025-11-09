using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions
{
    public record class Mission(long StartTime, long Duration, Vector2 Position, MissionContent Content);
}

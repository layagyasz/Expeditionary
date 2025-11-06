using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions
{
    public record class Mission(Vector2 Position, MissionContent Content);
}

using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public record class LineOfSight(Vector3i Target, int Distance, bool IsBlocked);
}

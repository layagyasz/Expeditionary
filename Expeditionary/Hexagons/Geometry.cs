using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Geometry
    {
        public static Vector3i GetEdge(Vector3i left, Vector3i right)
        {
            return left + right;
        }
    }
}

using OpenTK.Mathematics;
using System.Security.Cryptography.X509Certificates;

namespace Expeditionary.Hexagons
{
    public static class Geometry
    {
        private static readonly float s_Sqrt3 = MathF.Sqrt(3);

        private static readonly Vector3i[] s_HexCorners =
        {
            new(0, 0, 1),
            new(0, -1, 0),
            new(1, 0, 0),
            new(0, 0, -1),
            new(0, 1, 0),
            new(-1, 0, 0)
        };
        private static readonly Vector3i[] s_HexNeighbors =
        {
            new(0, -1, 1),
            new(1, -1, 0),
            new(1, 0, -1),
            new(0, 1, -1),
            new(-1, 1, 0),
            new(-1, 0, 1)
        };
        private static readonly Vector3i[] s_CornerHexes =
        {
            new(-1, 0, 0),
            new(0, -1, 0),
            new(0, 0, -1)
        };
        private static readonly Vector3i[] s_CornerNeighbors =
        {
            new(-1, -1, 0),
            new(0, -1, -1),
            new(-1, 0, -1)
        };


        public static Vector2 MapAxial(Vector2i axial)
        {
            return new(1.5f * axial.X, 0.5f * s_Sqrt3 * axial.X + s_Sqrt3 * axial.Y);
        }

        public static Vector2 MapOffset(Vector2i offset)
        {
            return MapAxial(Axial.Offset.Instance.Wrap(offset));
        }

        public static Vector3i GetCorner(Vector3i hex, int index)
        {
            return hex + s_HexCorners[index];
        }

        public static IEnumerable<Vector3i> GetCornerEdges(Vector3i corner)
        {
            int dir = GetCornerDirection(corner);
            return s_CornerNeighbors.Select(x => 2 * corner + dir * x);
        }

        public static Vector3i GetCornerHex(Vector3i corner, int index)
        {
            return corner + GetCornerDirection(corner) * s_CornerHexes[index];
        }

        public static IEnumerable<Vector3i> GetCornerHexes(Vector3i corner)
        {
            int dir = GetCornerDirection(corner);
            return s_CornerHexes.Select(x => corner + dir * x);
        }

        public static IEnumerable<Vector3i> GetCornerNeighbors(Vector3i corner)
        {
            int dir = GetCornerDirection(corner);
            return s_CornerNeighbors.Select(x => corner + dir * x);
        }

        public static int GetCornerDirection(Vector3i corner)
        {
            return corner.X + corner.Y + corner.Z;
        }

        public static Vector3i GetEdge(Vector3i left, Vector3i right)
        {
            return left + right;
        }

        public static IEnumerable<Vector3i> GetEdges(Vector3i hex)
        {
            return s_HexNeighbors.Select(x => 2 * hex + x);
        }

        public static IEnumerable<Vector3i> GetNeighbors(Vector3i hex)
        {
            return s_HexNeighbors.Select(x => hex + x);
        }
    }
}

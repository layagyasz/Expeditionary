using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Geometry
    {
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

        public static int GetDistance(Vector3i hexA, Vector3i hexB)
        {
            return Math.Max(
                Math.Abs(hexA.X - hexB.X), Math.Max(Math.Abs(hexA.Y - hexB.Y), Math.Abs(hexA.Z - hexB.Z)));
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

        public static Vector3i GetNeighbor(Vector3i hex, int neighbor)
        {
            return hex + s_HexNeighbors[neighbor];
        }

        public static Vector3i RoundHex(Vector3 hex)
        {
            var q = (int)Math.Round(hex.X);
            var r = (int)Math.Round(hex.Y);
            var s = (int)Math.Round(hex.Z);

            var dQ = Math.Abs(q - hex.X);
            var dR = Math.Abs(r - hex.Y);
            var dS = Math.Abs(s - hex.Z);

            if (dQ > dR && dQ > dS)
            {
                q = -r - s;
            }
            else if (dR > dS)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }
            return new(q, r, s);
        }
    }
}

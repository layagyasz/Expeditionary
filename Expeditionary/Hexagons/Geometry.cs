using Expeditionary.Model.Mapping;
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
        private static readonly int[] s_HexCornerIndixes = { 2, 1, 0, 2, 1, 0 };
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

        public static IEnumerable<Vector3i> GetAllCorners(Vector2i size)
        {
            for (int i = 0; i < size.X + 2; ++i)
            {
                for (int j = 0; j < 2 * size.Y + 2; ++j)
                {
                    yield return Cubic.TriangularOffset.Instance.Wrap(new(i, j));
                }
            }
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

        public static IEnumerable<(Vector3i, int)> GetCorners(Vector3i hex)
        {
            return Enumerable.Zip(s_HexCorners.Select(x => hex + x), s_HexCornerIndixes);
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

        public static int GetDisplacement(Vector3i delta)
        {
            return Math.Max(Math.Abs(delta.X), Math.Max(Math.Abs(delta.Y), Math.Abs(delta.Z)));
        }

        public static float GetCartesianDistance(Vector3i hexA, Vector3i hexB)
        {
            return Vector2.Distance(Cubic.Cartesian.Instance.Project(hexA), Cubic.Cartesian.Instance.Project(hexB));
        }

        public static int GetCubicDistance(Vector3i hexA, Vector3i hexB)
        {
            return GetDisplacement(hexB - hexA);
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

        public static IEnumerable<Vector3i> GetRange(Vector3i hex, int range)
        {
            for (int q = -range; q <= range; ++q)
            {
                for (int r = Math.Max(-range, -q - range); r <= Math.Min(range, -q + range); ++r)
                {
                    yield return hex + new Vector3i(q, r, -q - r);
                }
            }
        }

        public static Vector3i SnapToHex(Vector3 hex)
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

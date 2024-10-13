using Cardamom.Collections;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public static class RiverGenerator
    {
        public static void Generate(int count, Map map, float[,] corners, Random random)
        {
            for (int i = 0; i < count; ++i)
            {
                Trace(SelectStartCorner(map, random), map, corners);
            }
        }

        private static Vector3i SelectStartCorner(Map map, Random random)
        {
            while (true)
            {
                var coord = new Vector2i(random.Next(map.Width), random.Next(map.Height));
                if (map.GetTile(coord) == null)
                {
                    continue;
                }
                var hex = Cubic.HexagonalOffset.Instance.Wrap(coord);
                Vector3i corner = Geometry.GetCorner(hex, random.Next(6));
                if (!Geometry.GetCornerHexes(corner)
                    .Select(map.GetTile)
                    .Where(x => x != null)
                    .Any(x => x!.Terrain.IsLiquid))
                {
                    return corner;
                }
            }
        }

        private static void Trace(Vector3i start, Map map, float[,] corners)
        {
            Vector3i pos = start;
            Vector2i index = Cubic.TriangularOffset.Instance.Project(start);
            float elevation = corners[index.X, index.Y];
            ArrayList<Vector3i> edges = new();
            while (true)
            {
                var next = SelectNext(pos, map, corners);
                if (next == default)
                {
                    break;
                }
                var newEdge = Geometry.GetEdge(pos, next);
                if (map.GetEdge(newEdge) == null)
                {
                    break;
                }
                edges.Add(newEdge);
                var nextIndex = Cubic.TriangularOffset.Instance.Project(next);
                if (corners[nextIndex.X, nextIndex.Y] >= elevation)
                {
                    var tile = Geometry.GetCornerHexes(next)
                        .Select(map.GetTile)
                        .Where(x => x != null)
                        .ArgMin(x => x!.Elevation);
                    tile!.Terrain.IsLiquid = true;
                    break;
                }
                if (Geometry.GetCornerHexes(pos)
                    .Select(map.GetTile)
                    .Where(x => x != null)
                    .Any(x => x!.Terrain.IsLiquid))
                {
                    break;
                }
                pos = next;
                index = nextIndex;
                elevation = corners[nextIndex.X, nextIndex.Y];
            }
            foreach (var edge in edges)
            {
                map.GetEdge(edge)!.Levels.Add(EdgeType.River, 1);
            }
        }

        private static Vector3i SelectNext(Vector3i pos, Map map, float[,] corners)
        {
            foreach (var neighbor in Geometry.GetCornerNeighbors(pos))
            {
                foreach (var edge in Geometry.GetCornerEdges(neighbor))
                {
                    if (edge != Geometry.GetEdge(pos, neighbor) 
                        && (map.GetEdge(edge)?.Levels?.ContainsKey(EdgeType.River) ?? false))
                    {
                        return neighbor;
                    }
                }
            }
            var normalNext = Geometry.GetCornerNeighbors(pos)
                .Select(Cubic.TriangularOffset.Instance.Project)
                .Where(x => x.X >= 0 && x.Y >= 0 && x.X < corners.GetLength(0) && x.Y < corners.GetLength(1))
                .ArgMin(x => corners[x.X, x.Y]);
            return Cubic.TriangularOffset.Instance.Wrap(normalNext);
        }
    }
}

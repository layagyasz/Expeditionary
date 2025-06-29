﻿using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public record class EdgeMapRegion(MapDirection Edge, float Extent) : IMapRegion
    {
        public bool Contains(Map map, Vector3i hex)
        {
            var offset = Hexagons.Cubic.HexagonalOffset.Instance.Project(hex);
            return GetBoxes(map, Edge, Extent).Any(x => x.ContainsInclusive(offset));
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            var boxes = GetBoxes(map, Edge, Extent).ToList();
            if (boxes.Count == 0)
            {
                return Enumerable.Empty<Vector3i>();
            }
            if (boxes.Count == 1)
            {
                return EnumerateBox(boxes[0]);
            }
            return map.Range().Where(x => Contains(map, x));
        }

        private IEnumerable<Vector3i> EnumerateBox(Box2i box)
        {
            for (int i = box.Min.X; i <= box.Max.X; ++i)
            {
                for (int j = box.Min.Y; j <= box.Max.Y; ++j)
                {
                    yield return Hexagons.Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
                }
            }
        }

        private static IEnumerable<Box2i> GetBoxes(Map map, MapDirection edge, float extent)
        {
            return Enum.GetValues<MapDirection>().Where(x => edge.HasFlag(x)).Select(x => GetBox(map, x, extent));
        }

        private static Box2i GetBox(Map map, MapDirection edge, float extent) => edge switch
        {
            MapDirection.North => new(0, 0, map.Width - 1, (int)(extent * map.Height)),
            MapDirection.East => new((int)((1 - extent) * map.Width), 0, map.Width - 1, map.Height - 1),
            MapDirection.South => new(0, (int)((1 - extent) * map.Height), map.Width - 1, map.Height - 1),
            MapDirection.West => new(0, 0, (int)(extent * map.Width), map.Height - 1),
            _ => new()
        };
    }
}

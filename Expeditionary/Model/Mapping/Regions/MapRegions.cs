using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public static class MapRegions
    {
        public static readonly IMapRegion Empty = new ExplicitMapRegion(Enumerable.Empty<Vector3i>());

        public static bool Intersects(IMapRegion left, IMapRegion right, Map map)
        {
            return left.Range(map).Any(x => right.Contains(map, x));
        }

        public static IEnumerable<IMapRegion> Partition(
            this IMapRegion region, Map map, Vector2 keyVector, int partitions)
        {
            var tiles = region.Range(map).ToArray();
            var keys = 
                tiles.Select(Hexagons.Cubic.HexagonalOffset.Instance.Project)
                    .Select(x => Vector2.Dot(keyVector, x))
                    .ToArray();
            Array.Sort(keys, tiles);
            for (int i=0; i<partitions; i++) 
            {
                int left = i * tiles.Length / partitions;
                int right = (i + 1) * tiles.Length / partitions;
                yield return new ExplicitMapRegion(new ArraySegment<Vector3i>(tiles, left, right - left));
            }
        }
    }
}

using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.Considerations
{
    public static class TileConsiderations
    {
        public static float Evaluate(TileConsideration consideration, Vector3i hex, Map map)
        {
            var tile = map.Get(hex)!;
            if (tile.Terrain.IsLiquid)
            {
                return -1;
            }
            return consideration.Invoke(hex, tile);
        }

        public static TileConsideration Combine(params TileConsideration[] considerations)
        {
            return (hex, tile) => considerations.Sum(x => x.Invoke(hex, tile));
        }

        public static TileConsideration Weight(float weight, TileConsideration consideration)
        {
            return (hex, tile) => weight * consideration.Invoke(hex, tile);
        }

        public static TileConsideration Exposure(
            ExposureCache cache, MapDirection facing, Disposition disposition, RangeBucket range)
        {
            return (hex, tile) => cache.Evaluate(hex, facing, disposition, range);
        }

        public static float Forestation(Vector3i _, Tile tile)
        {
            return tile.Terrain.Foliage != null ? 1 : 0;
        }

        public static TileConsideration Noise(Random random)
        {
            return (_, _) => random.NextSingle();
        }

        public static TileConsideration Roading(Map map)
        {
            return (hex, tile) => 
                0.1666666667f * Geometry.GetEdges(hex)
                    .Select(map.GetEdge)
                    .Where(x => x != null)
                    .Count(x => x!.Levels[EdgeType.Road] > 0);
        }

        public static float Urbanization(Vector3i _, Tile tile)
        {
            return tile.IsUrban() ? 1 : 0;
        }
    }
}

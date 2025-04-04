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
            return consideration(hex, tile);
        }

        public static TileConsideration Combine(params TileConsideration[] considerations)
        {
            return (hex, tile) =>
            {
                var score = 0f;
                foreach (var consideration in considerations)
                {
                    score += consideration(hex, tile);
                    if (float.IsInfinity(score))
                    {
                        return score;
                    }
                }
                return score;
            };
        }

        public static TileConsideration Distance(SignedDistanceField field, int frontier, int median)
        {
            median = MathHelper.Clamp(median, -field.MaxInternalDistance + 1, frontier - 1);
            return (hex, _) =>
            {
                var rawDistance = (float)field.Get(hex) - median;
                if (rawDistance < 0)
                {
                    return 0.5f - rawDistance / (field.MaxInternalDistance + median);
                }
                else
                {
                    if (rawDistance > frontier - median)
                    {
                        return float.PositiveInfinity;
                    }
                    return 0.5f + rawDistance / (frontier - median);
                }
            };
        }

        public static TileConsideration Edge(TileConsideration consideration)
        {
            return (hex, tile) => 1 - 2 * Math.Abs(0.5f - consideration(hex, tile));
        }

        public static TileConsideration Essential(TileConsideration consideration)
        {
            return (hex, tile) =>
            {
                var score = consideration(hex, tile);
                if (score < float.Epsilon)
                {
                    return float.NegativeInfinity;
                }
                return score;
            };
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

        public static TileConsideration Inverse(TileConsideration consideration)
        {
            return (hex, tile) => 1 - consideration(hex, tile);
        }

        public static float Land(Vector3i _, Tile tile)
        {
            return tile.Terrain.IsLiquid ? 0 : 1;
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

        public static TileConsideration Weight(float weight, TileConsideration consideration)
        {
            return (hex, tile) => weight * consideration.Invoke(hex, tile);
        }

    }
}

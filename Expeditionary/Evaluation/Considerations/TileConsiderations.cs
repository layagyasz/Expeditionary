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
                    if (float.IsNaN(score))
                    {
                        throw new ApplicationException();
                    }
                    if (float.IsInfinity(score))
                    {
                        return score;
                    }
                }
                return score;
            };
        }

        public static TileConsideration Edge(SignedDistanceField sdf, int offset)
        {
            return (hex, _) => 1 - Math.Abs(SdfRelativeDistance(sdf, hex, offset));
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
            return (hex, _) => cache.Evaluate(hex, facing, disposition, range);
        }

        public static TileConsideration Exterior(SignedDistanceField sdf, int offset)
        {
            return (hex, _) => Math.Max(0, SdfRelativeDistance(sdf, hex, offset));
        }

        public static float Forestation(Vector3i _, Tile tile)
        {
            return tile.Terrain.Foliage != null ? 1 : 0;
        }

        public static TileConsideration Interior(SignedDistanceField sdf, int offset)
        {
            return (hex, _) => Math.Max(0, -SdfRelativeDistance(sdf, hex, offset));
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

        private static float SdfRelativeDistance(SignedDistanceField sdf, Vector3i hex, int offset) 
        {
            var rawDistance = (float)sdf.Get(hex) - offset;
            if (rawDistance < 0)
            {
                if (sdf.MaxInternalDistance + offset > 0)
                {
                    return Check(Math.Max(-1, rawDistance / (sdf.MaxInternalDistance + offset)));
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (sdf.MaxExternalDistance - offset > 0)
                {
                    return Check(Math.Min(1, rawDistance / (sdf.MaxExternalDistance - offset)));
                }
                else
                {
                    return 1;
                }
            }
        }

        private static float Check(float value)
        {
            if (float.IsNaN(value))
            {
                throw new ApplicationException();
            }
            return value;
        }
    }
}

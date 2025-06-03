using Expeditionary.Evaluation.SignedDistanceFields;
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
            return consideration(
                hex,
                () => tile, 
                () => Geometry.GetEdges(hex).Select(map.GetEdge).Where(x => x != null).Cast<Edge>());
        }

        public static TileConsideration Combine(params TileConsideration[] considerations)
        {
            return (hex, tileFn, edgesFn) =>
            {
                var score = 0f;
                foreach (var consideration in considerations)
                {
                    score += consideration(hex, tileFn, edgesFn);
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

        public static TileConsideration Direction(Vector3i origin, MapDirection direction)
        {
            return (hex, _, _) => (MapDirectionUtils.GetInclusiveDirection(origin, hex) & direction) > 0 ? 1 : 0;
        }

        public static TileConsideration Edge(ISignedDistanceField sdf, int offset)
        {
            return (hex, _, _) => 1 - Math.Abs(SdfRelativeDistance(sdf, hex, offset));
        }

        public static TileConsideration Essential(TileConsideration consideration)
        {
            return (hex, tileFn, edgesFn) =>
            {
                var score = consideration(hex, tileFn, edgesFn);
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
            return (hex, _, _) => cache.Evaluate(hex, facing, disposition, range);
        }

        public static TileConsideration Exterior(ISignedDistanceField sdf, int offset)
        {
            return (hex, _, _) => Math.Max(0, SdfRelativeDistance(sdf, hex, offset));
        }

        public static float Forestation(Vector3i _0, Func<Tile> tileFn, Func<IEnumerable<Edge>> _2)
        {
            return tileFn().Terrain.Foliage != null ? 1 : 0;
        }

        public static TileConsideration Interior(ISignedDistanceField sdf, int offset)
        {
            return (hex, _, _) => Math.Max(0, -SdfRelativeDistance(sdf, hex, offset));
        }

        public static TileConsideration Inverse(TileConsideration consideration)
        {
            return (hex, tileFn, edgesFn) => 1 - consideration(hex, tileFn, edgesFn);
        }

        public static float Land(Vector3i _0, Func<Tile> tileFn, Func<IEnumerable<Edge>> _2)
        {
            return tileFn().Terrain.IsLiquid ? 0 : 1;
        }

        public static TileConsideration Noise(Random random)
        {
            return (_, _, _) => random.NextSingle();
        }

        public static float None(Vector3i _0, Func<Tile> _1, Func<IEnumerable<Edge>> _2)
        {
            return 0;
        }

        public static float Roading(Vector3i hex, Func<Tile> tileFn, Func<IEnumerable<Edge>> edgesFn)
        {
            return 0.1666666667f * edgesFn()
                .Where(x => x != null)
                .Count(x => x!.Levels[EdgeType.Road] > 0);
        }

        public static float Urbanization(Vector3i _0, Func<Tile> tileFn, Func<IEnumerable<Edge>> _2)
        {
            return tileFn().IsUrban() ? 1 : 0;
        }

        public static TileConsideration Weight(float weight, TileConsideration consideration)
        {
            return (hex, tileFn, edgesFn) => weight * consideration.Invoke(hex, tileFn, edgesFn);
        }

        private static float SdfRelativeDistance(ISignedDistanceField sdf, Vector3i hex, int offset) 
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

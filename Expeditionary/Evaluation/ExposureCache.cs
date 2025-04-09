using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation
{
    public class ExposureCache
    {
        private record CacheKey(Vector3i Hex, MapDirection Facing, RangeBucket Range);

        public Map Map { get; }

        private readonly Dictionary<CacheKey, float> _exposureCache = new();

        public ExposureCache(Map map)
        {
            Map = map;
        }

        public float Evaluate(UnitType unitType, Vector3i hex, MapDirection facing, Disposition disposition)
        {
            var range = disposition == Disposition.Offensive ? RangeBucketizer.ToBucket(unitType) : RangeBucket.Medium;
            return Evaluate(hex, facing, disposition, range);
        }

        public float Evaluate(Vector3i hex, MapDirection facing, Disposition disposition, RangeBucket range)
        {
            var exp = GetOrEvaluateExposure(new CacheKey(hex, facing, range));
            if (disposition == Disposition.Defensive)
            {
                return 1 - exp;
            }
            return exp;
        }

        private float GetOrEvaluateExposure(CacheKey key)
        {
            if (_exposureCache.TryGetValue(key, out var exp))
            {
                return exp;
            }
            exp = EvaluateExposure(key.Hex, Map, key.Facing, RangeBucketizer.ToRange(key.Range));
            _exposureCache.Add(key, exp);
            return exp;
        }

        public static float EvaluateExposure(
            Vector3i hex, Map map, MapDirection direction, int maxRange)
        {
            var c = DirectionCoefficient(direction);
            return c * Sighting.GetSightField(map, hex, maxRange)
                .Where(x => !x.IsBlocked)
                .Where(x => !map.Get(x.Target)?.Terrain?.IsLiquid ?? false)
                .Where(x => (direction & MapDirectionUtils.GetExclusiveDirection(hex, x.Target)) > 0)
                .Count() / (3f * maxRange * (maxRange + 1));
        }

        private static float DirectionCoefficient(MapDirection direction)
        {
            return 5
                - Convert.ToSingle(direction.HasFlag(MapDirection.North))
                - Convert.ToSingle(direction.HasFlag(MapDirection.South))
                - Convert.ToSingle(direction.HasFlag(MapDirection.East))
                - Convert.ToSingle(direction.HasFlag(MapDirection.West));
        }
    }
}

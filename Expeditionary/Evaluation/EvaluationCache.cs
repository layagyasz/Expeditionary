using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation
{
    public class EvaluationCache
    {
        private record CacheKey(Vector3i Hex, MapDirection Facing, RangeBucket Range);

        public Map Map { get; }

        private readonly Dictionary<CacheKey, float> _exposureCache = new();

        public EvaluationCache(Map map)
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
            var tile = Map.Get(hex)!;
            if (tile.Terrain.IsLiquid)
            {
                return -1;
            }

            var exp = GetOrEvaluateExposure(new CacheKey(hex, facing, range));
            if (disposition == Disposition.Defensive)
            {
                exp = 1 - exp;
            }
            var def = TileEvaluation.EvaluateDefensibility(tile);
            return exp + def;

        }

        private float GetOrEvaluateExposure(CacheKey key)
        {
            if (_exposureCache.TryGetValue(key, out var exp))
            {
                return exp;
            }
            exp = TileEvaluation.EvaluateExposure(key.Hex, Map, key.Facing, RangeBucketizer.ToRange(key.Range));
            _exposureCache.Add(key, exp);
            return exp;
        }
    }
}

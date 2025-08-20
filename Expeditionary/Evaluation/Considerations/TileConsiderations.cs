using Expeditionary.Evaluation.Caches;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
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
            return Combine(considerations.Select(x => (1f, x)).ToArray());
        }

        public static TileConsideration Combine(params (float, TileConsideration)[] considerations)
        {
            var total = 1f / considerations.Sum(x => x.Item1);
            return (hex, tileFn, edgesFn) =>
            {
                var score = 0f;
                foreach ((var weight, var consideration) in considerations)
                {
                    score += weight * consideration(hex, tileFn, edgesFn);
                    if (float.IsNaN(score))
                    {
                        throw new ApplicationException();
                    }
                    if (float.IsInfinity(score))
                    {
                        return score;
                    }
                }
                return total * score;
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

        public static TileConsideration IsReachable(
            PartitionCache cache, Movement.Hindrance maxHindrance, Vector3i origin)
        {
            return (hex, _, _) => cache.IsReachable(origin, hex, maxHindrance) ? 1 : 0;
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

        public static float Roading(Vector3i _0, Func<Tile> _1, Func<IEnumerable<Edge>> edgesFn)
        {
            return 0.1666666667f * edgesFn()
                .Where(x => x != null)
                .Count(x => x!.Levels[EdgeType.Road] > 0);
        }

        public static TileConsideration Subtract(TileConsideration left, TileConsideration right)
        {
            return (hex, tileFn, edgesFn) => left(hex, tileFn, edgesFn) - right(hex, tileFn, edgesFn);
        }

        public static TileConsideration Threat(Unit unit, IPlayerKnowledge knowledge, Match match)
        {
            var attackers = 
                match.GetAssets()
                    .Where(x => x is Unit)
                    .Cast<Unit>()
                    .Where(x => CombatCalculator.IsValidTarget(x, unit))
                    .Select(x => (x, knowledge.GetAsset(x)))
                    .Where(x => x.Item2.IsVisible && x.Item2.LastSeen != null)
                    .ToList();
            var map = match.GetMap();
            return (hex, tileFn, edgesFn) =>
            {
                var total = 0f;
                foreach ((var attacker, var assetKnowledge) in attackers)
                {
                    var attackerPosition = assetKnowledge.LastSeen!.Value;
                    var potential =
                        attacker.Type.Weapons
                            .SelectMany(Weapon => Weapon.Weapon.Modes.Select(Mode => (Weapon, Mode)))
                            .Where(x => 
                                CombatCalculator.IsValidTarget(attacker, attackerPosition, x.Mode, unit, hex, map))
                            .Select(x => 
                                CombatCalculator.GetDirectPreview(
                                    attacker, attackerPosition, x.Weapon, x.Mode, unit, hex, map).Result)
                            .DefaultIfEmpty(0)
                            .Max();
                    total += potential;
                }
                return Math.Min(1f, total / unit.Number);
            };
        }

        public static float Urbanization(Vector3i _0, Func<Tile> tileFn, Func<IEnumerable<Edge>> _2)
        {
            return tileFn().IsUrban() ? 1 : 0;
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

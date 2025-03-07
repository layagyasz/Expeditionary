using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation
{
    public static class TileEvaluation
    {
        public static float Evaluate(
            Vector3i hex, Map map, Disposition disposition, MapDirection direction, UnitType unitType)
        {
            return Evaluate(
                hex, 
                map,
                disposition,
                direction,
                disposition == Disposition.Offensive
                    ? (int)unitType.Weapons.SelectMany(x => x.Weapon!.Modes).Select(x => x.Range.Get()).Max() 
                    : 4);
        }

        public static float Evaluate(Vector3i hex, Map map, Disposition disposition, MapDirection direction, int maxRange)
        {
            var tile = map.Get(hex)!;
            if (tile.Terrain.IsLiquid)
            {
                return -1;
            }
            var c = DirectionCoefficient(direction);
            var exp = c * EvaluateExposure(hex, map, direction, maxRange);
            if (disposition == Disposition.Defensive)
            {
                exp = 1 - exp;
            }
            var def = EvaluateDefensibility(map.Get(hex)!);
            return exp + def;
        }

        private static float EvaluateExposure(
            Vector3i hex, Map map, MapDirection direction,  int maxRange)
        {
            return 1f * Sighting.GetSightField(map, hex, maxRange)
                .Where(x => !x.IsBlocked)
                .Where(x => !map.Get(x.Target)?.Terrain?.IsLiquid ?? false)
                .Where(x => DirectionContains(Geometry.GetCartesianDisplacement(hex, x.Target), direction))
                .Count()
                / (3f * maxRange * (maxRange + 1));
        }

        private static float EvaluateDefensibility(Tile tile)
        {
            return tile.Terrain.Foliage != null || tile.IsUrban() ? 1 : 0;
        }

        private static float DirectionCoefficient(MapDirection direction)
        {
            return 5 
                - Convert.ToSingle(direction.HasFlag(MapDirection.North)) 
                - Convert.ToSingle(direction.HasFlag(MapDirection.South))
                - Convert.ToSingle(direction.HasFlag(MapDirection.East))
                - Convert.ToSingle(direction.HasFlag(MapDirection.West));
        }

        private static bool DirectionContains(Vector2 displacement, MapDirection direction)
        {

            if (direction.HasFlag(MapDirection.North) 
                && displacement.Y <= 0 
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return true;
            }
            if (direction.HasFlag(MapDirection.South)
                && displacement.Y >= 0
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return true;
            }
            if (direction.HasFlag(MapDirection.East)
                && displacement.X >= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return true;
            }
            if (direction.HasFlag(MapDirection.West)
                && displacement.X <= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return true;
            }
            return false;
        }
    }
}

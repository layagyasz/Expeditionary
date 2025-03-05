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
            Vector3i hex, Map map, Disposition disposition, Direction direction, UnitType unitType)
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

        public static float Evaluate(Vector3i hex, Map map, Disposition disposition, Direction direction, int maxRange)
        {
            var tile = map.GetTile(hex)!;
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
            var def = EvaluateDefensibility(map.GetTile(hex)!);
            return exp + def;
        }

        private static float EvaluateExposure(
            Vector3i hex, Map map, Direction direction,  int maxRange)
        {
            return 1f * Sighting.GetSightField(map, hex, maxRange)
                .Where(x => !x.IsBlocked)
                .Where(x => !map.GetTile(x.Target)?.Terrain?.IsLiquid ?? false)
                .Where(x => DirectionContains(Geometry.GetCartesianDisplacement(hex, x.Target), direction))
                .Count()
                / (3f * maxRange * (maxRange + 1));
        }

        private static float EvaluateDefensibility(Tile tile)
        {
            return tile.Terrain.Foliage != null || tile.IsUrban() ? 1 : 0;
        }

        private static float DirectionCoefficient(Direction direction)
        {
            return 5 
                - Convert.ToSingle(direction.HasFlag(Direction.North)) 
                - Convert.ToSingle(direction.HasFlag(Direction.South))
                - Convert.ToSingle(direction.HasFlag(Direction.East))
                - Convert.ToSingle(direction.HasFlag(Direction.West));
        }

        private static bool DirectionContains(Vector2 displacement, Direction direction)
        {

            if (direction.HasFlag(Direction.North) 
                && displacement.Y <= 0 
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return true;
            }
            if (direction.HasFlag(Direction.South)
                && displacement.Y >= 0
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return true;
            }
            if (direction.HasFlag(Direction.East)
                && displacement.X >= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return true;
            }
            if (direction.HasFlag(Direction.West)
                && displacement.X <= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return true;
            }
            return false;
        }
    }
}

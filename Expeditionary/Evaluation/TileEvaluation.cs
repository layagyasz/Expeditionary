using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation
{
    public static class TileEvaluation
    {
        public static float EvaluateExposure(
            Vector3i hex, Map map, MapDirection direction, int maxRange)
        {
            var c = DirectionCoefficient(direction);
            return c * Sighting.GetSightField(map, hex, maxRange)
                .Where(x => !x.IsBlocked)
                .Where(x => !map.Get(x.Target)?.Terrain?.IsLiquid ?? false)
                .Where(x => DirectionContains(Geometry.GetCartesianDisplacement(hex, x.Target), direction))
                .Count()
                / (3f * maxRange * (maxRange + 1));
        }

        public static float EvaluateDefensibility(Tile tile)
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

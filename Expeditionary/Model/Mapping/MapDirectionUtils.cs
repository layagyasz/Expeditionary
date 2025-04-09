using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public static class MapDirectionUtils
    {
        public static MapDirection Invert(MapDirection direction)
        {
            MapDirection newDirection = 0;
            if (direction.HasFlag(MapDirection.North))
            {
                newDirection |= MapDirection.South;
            }
            if (direction.HasFlag(MapDirection.East))
            {
                newDirection |= MapDirection.West;
            }
            if (direction.HasFlag(MapDirection.South))
            {
                newDirection |= MapDirection.North;
            }
            if (direction.HasFlag(MapDirection.West))
            {
                newDirection |= MapDirection.East;
            }
            return newDirection;
        }

        public static MapDirection GetExclusiveDirection(Vector3i left, Vector3i right)
        {
            var displacement = Geometry.GetCartesianDisplacement(left, right);
            if (displacement.Y <= 0
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return MapDirection.North;
            }
            if (displacement.Y >= 0
                && Math.Abs(displacement.Y) >= Math.Abs(displacement.X))
            {
                return MapDirection.South;
            }
            if (displacement.X >= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return MapDirection.East;
            }
            if (displacement.X <= 0
                && Math.Abs(displacement.X) >= Math.Abs(displacement.Y))
            {
                return MapDirection.West;
            }
            return 0;
        }

        public static MapDirection GetInclusiveDirection(Vector3i left, Vector3i right)
        {
            var displacement = right - left;
            MapDirection result = 0;
            if (displacement.X < 0)
            {
                result |= MapDirection.West;
            }
            if (displacement.X > 0)
            {
                result |= MapDirection.East;
            }
            if (displacement.Z > displacement.Y)
            {
                result |= MapDirection.North;
            }
            if (displacement.Y > displacement.Z)
            {
                result |= MapDirection.South;
            }
            return result;
        }
    }
}

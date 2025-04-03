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
    }
}

using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public static class Sighting
    {
        private static readonly float s_FoliageHeight = 0.4f;
        private static readonly float s_StructureHeight = 0.4f;

        public static LineOfSight GetLineOfSight(Map map, Vector3i position, Vector3i target)
        {
            return new(
                target, 
                Geometry.GetCubicDistance(target, position), 
                !IsValidLineOfSightInternal(map, position, target));
        }

        public static IEnumerable<LineOfSight> GetSightField(Map map, Vector3i position, int range)
        {
            foreach (var target in Geometry.GetRange(position, range))
            {
                yield return GetLineOfSight(map, position, target);
            }
        }

        public static IEnumerable<LineOfSight> GetUnblockedSightField(Map map, Vector3i position, int range)
        {
            return GetSightField(map, position, range).Where(x => !x.IsBlocked);
        }

        public static bool IsValidLineOfSight(Map map, Vector3i position, Vector3i target)
        {
            if (position == target)
            {
                return true;
            }
            return IsValidLineOfSightInternal(map, position, target);
        }

        private static bool IsValidLineOfSightInternal(Map map, Vector3i position, Vector3i target)
        {
            var distance = Geometry.GetCubicDistance(target, position);
            if (distance < 2)
            {
                return true;
            }

            var start = map.GetTile(position);
            if (start == null)
            {
                return false;
            }

            var end = map.GetTile(target);
            if (end == null)
            {
                return false;
            }
            
            var step = (Vector3)(target - position) / distance;
            var slope = (end.Elevation - start.Elevation) / distance;
            for (int i=1; i<distance; ++i)
            {
                var hex = Geometry.SnapToHex(position + i * step);
                var tile = map.GetTile(hex);
                if (tile == null)
                {
                    return false;
                }
                var foliage = tile.Terrain.Foliage == null ? 0 : s_FoliageHeight;
                var structure = tile.Structure.Level == 0 ? 0 : s_StructureHeight;
                if (tile.Elevation + Math.Max(foliage, structure) > start.Elevation + i * slope)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

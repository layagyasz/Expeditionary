using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public record class PointMapRegion(Vector3i Hex, int Distance) : IMapRegion
    {
        public bool Contains(Map map, Vector3i hex)
        {
            return Geometry.GetCubicDistance(Hex, hex) <= Distance;
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            return Geometry.GetRange(Hex, Distance).Where(map.Contains);
        }
    }
}

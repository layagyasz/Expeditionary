using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public record class EntireMapRegion : IMapRegion
    {
        public bool Contains(Map map, Vector3i hex)
        {
            return true;
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            return map.Range();
        }
    }
}

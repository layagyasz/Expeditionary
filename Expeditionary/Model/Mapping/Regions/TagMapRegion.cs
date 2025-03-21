using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public record class TagMapRegion(MapTag Tag) : IMapRegion
    {
        public bool Contains(Map map, Vector3i hex)
        {
            return map.Get(hex)?.Tags?.Contains(Tag) ?? false;
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            return map.GetArea(Tag);
        }
    }
}

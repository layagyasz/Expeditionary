using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public interface IMapRegion
    {
        bool Contains(Map map, Vector3i hex);
        IEnumerable<Vector3i> Range(Map map);
    }
}

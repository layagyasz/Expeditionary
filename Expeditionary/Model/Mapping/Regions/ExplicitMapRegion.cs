using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Regions
{
    public class ExplicitMapRegion : IMapRegion
    {
        private readonly HashSet<Vector3i> _range;

        public ExplicitMapRegion(IEnumerable<Vector3i> range)
        {
            _range = new HashSet<Vector3i>(range);
        }

        public bool Contains(Map map, Vector3i hex)
        {
            return _range.Contains(hex);
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            return _range;
        }
    }
}

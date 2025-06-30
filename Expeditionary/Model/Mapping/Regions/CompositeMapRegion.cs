using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Expeditionary.Model.Mapping.Regions
{
    public class CompositeMapRegion : IMapRegion
    {
        public enum Operation
        {
            Union,
            Intersection,
        }

        private readonly Operation _operation;
        private readonly ImmutableList<IMapRegion> _regions;

        private CompositeMapRegion(Operation operation, ImmutableList<IMapRegion> regions)
        {
            _operation = operation;
            _regions = regions;
        }

        public static CompositeMapRegion Intersect(params IMapRegion[] regions)
        {
            return new(Operation.Intersection, regions.ToImmutableList());
        }

        public static CompositeMapRegion Intersect(IEnumerable<IMapRegion> regions)
        {
            return new(Operation.Intersection, regions.ToImmutableList());
        }

        public static CompositeMapRegion Union(params IMapRegion[] regions)
        {
            return new(Operation.Union, regions.ToImmutableList());
        }

        public static CompositeMapRegion Union(IEnumerable<IMapRegion> regions)
        {
            return new(Operation.Union, regions.ToImmutableList());
        }

        public bool Contains(Map map, Vector3i hex)
        {
            if (!_regions.Any())
            {
                return false;
            }
            return _operation switch
            {
                Operation.Union => _regions.Any(x => x.Contains(map, hex)),
                Operation.Intersection => _regions.All(x => x.Contains(map, hex)),
                _ => throw new NotSupportedException(),
            };
        }

        public IEnumerable<Vector3i> Range(Map map)
        {
            if (!_regions.Any())
            {
                return Enumerable.Empty<Vector3i>();
            }
            ISet<Vector3i> range = _regions.First().Range(map).ToHashSet();
            foreach (var region in _regions.Skip(1))
            {
                switch (_operation)
                {
                    case Operation.Union:
                        range.UnionWith(region.Range(map));
                        break;
                    case Operation.Intersection:
                        range.IntersectWith(region.Range(map));
                        break;
                }
            }
            return range;
        }
    }
}

using Cardamom.Collections;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class MapKnowledge
    {
        private readonly Vector2i _size;
        private readonly IMapDiscovery _discovery;
        private readonly MultiMap<Vector3i, Unit> _sighters = new();

        public MapKnowledge(Vector2i size, IMapDiscovery discovery) 
        {
            _size = size;
            _discovery = discovery;
        }

        public SingleTileKnowledge Get(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            if (coord.X < 0 || coord.X >= _size.X || coord.Y < 0 || coord.Y >= _size.Y)
            {
                return new(IsDiscovered: false, IsVisible: false);
            }
            return new(_discovery.IsDiscovered(coord), _sighters.ContainsKey(hex));
        }


        public List<Vector3i> Move(Map map, IAsset asset, Pathing.Path path)
        {
            if (asset is not Unit unit)
            {
                return new();
            }

            var initialSightField = Sighting.GetSightField(map, unit, path.Origin).Select(x => x.Target).ToHashSet();
            var stepSightFields =
                path.Steps.Take(path.Steps.Count - 1)
                    .SelectMany(x => Sighting.GetSightField(map, unit, x))
                    .Select(x => x.Target)
                    .ToHashSet();
            var finalSightField =
                Sighting.GetSightField(map, unit, path.Destination).Select(x => x.Target).ToHashSet();
            stepSightFields.ExceptWith(initialSightField);
            stepSightFields.ExceptWith(finalSightField);

            var result = new List<Vector3i>();
            foreach (var hex in initialSightField)
            {
                if (_sighters.Remove(hex, unit))
                {
                    result.Add(hex);
                }
            }
            foreach (var hex in stepSightFields)
            {
                if (_discovery.Discover(Cubic.HexagonalOffset.Instance.Project(hex)))
                {
                    result.Add(hex);
                }
            }
            foreach (var hex in finalSightField)
            {
                _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(hex));
                _sighters.Add(hex, unit);
                result.Add(hex);
            }
            return result;
        }

        public List<Vector3i> Place(Map map, IAsset asset, Vector3i position)
        {
            if (asset is not Unit unit)
            {
                return new();
            }

            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(map, unit, position))
            {
                _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(los.Target));
                _sighters.Add(los.Target, unit);
                result.Add(los.Target);
            }
            return result;
        }

        public List<Vector3i> Remove(Map map, IAsset asset)
        {
            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(map, asset, asset.Position))
            {
                if (_sighters.Remove(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            return result;
        }
    }
}

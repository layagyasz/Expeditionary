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
        private readonly Map _map;
        private readonly IMapDiscovery _discovery;
        private readonly MultiMap<Vector3i, Unit> _spotters = new();

        public MapKnowledge(Map map, IMapDiscovery discovery) 
        {
            _map = map;
            _discovery = discovery;
        }

        public SingleTileKnowledge Get(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            if (coord.X < 0 || coord.X >= _map.Size.X || coord.Y < 0 || coord.Y >= _map.Size.Y)
            {
                return new(IsDiscovered: false, IsVisible: false);
            }
            var condition = _map.GetTile(hex)!.GetConditions();
            return new(
                _discovery.IsDiscovered(coord), 
                _spotters[hex]
                    .Any(x => x.Type.Capabilities.GetRange(condition, UnitDetectionBand.Visual).GetValue()
                        >= Geometry.GetCubicDistance(x.Position, hex)));
        }

        public EnumMap<UnitDetectionBand, float>? GetDetection(Vector3i hex)
        {
            var condition = _map.GetTile(hex)!.GetConditions();
            var spotters = _spotters[hex].ToList();
            if (!spotters.Any())
            {
                return null;
            }

            var result = new EnumMap<UnitDetectionBand, float>();
            foreach (var spotter in _spotters[hex])
            {
                foreach (var band in Enum.GetValues<UnitDetectionBand>())
                {
                    result[band] = 
                        Math.Max(result[band], SpottingCalculator.GetDetection(spotter, band, condition, hex));
                }
            }
            return result;
        }

        public Map GetMap()
        {
            return _map;
        }

        public List<Vector3i> Move(IAsset asset, Pathing.Path path)
        {
            if (asset is not Unit unit)
            {
                return new();
            }

            var initialSightField = Sighting.GetSightField(_map, unit, path.Origin).Select(x => x.Target).ToHashSet();
            var stepSightFields =
                path.Steps.Take(path.Steps.Count - 1)
                    .SelectMany(x => Sighting.GetSightField(_map, unit, x))
                    .Select(x => x.Target)
                    .ToHashSet();
            var finalSightField =
                Sighting.GetSightField(_map, unit, path.Destination).Select(x => x.Target).ToHashSet();
            stepSightFields.ExceptWith(initialSightField);
            stepSightFields.ExceptWith(finalSightField);

            var result = new List<Vector3i>();
            foreach (var hex in initialSightField)
            {
                if (_spotters.Remove(hex, unit))
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
                _spotters.Add(hex, unit);
                result.Add(hex);
            }
            return result;
        }

        public List<Vector3i> Place(IAsset asset, Vector3i position)
        {
            if (asset is not Unit unit)
            {
                return new();
            }

            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(_map, unit, position))
            {
                _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(los.Target));
                _spotters.Add(los.Target, unit);
                result.Add(los.Target);
            }
            return result;
        }

        public List<Vector3i> Remove(IAsset asset)
        {
            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(_map, asset, asset.Position))
            {
                if (_spotters.Remove(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            return result;
        }
    }
}

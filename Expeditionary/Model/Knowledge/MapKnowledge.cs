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
                        >= Geometry.GetCubicDistance(x.Position, hex) 
                        && Sighting.IsValidLineOfSight(_map, x.Position, hex)));
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
                var los = Sighting.GetLineOfSight(_map, spotter.Position, hex);
                foreach (var band in Enum.GetValues<UnitDetectionBand>())
                {
                    result[band] = 
                        Math.Max(result[band], SpottingCalculator.GetDetection(spotter, band, los, condition, hex));
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

            var maxRange = GetMaxRange(unit);
            var visualRange = GetVisualRange(unit);

            var initialLos = Sighting.GetSightField(_map, path.Origin, maxRange).ToHashSet();
            var stepLos =
                path.Steps.Take(path.Steps.Count - 1)
                    .SelectMany(x => Sighting.GetSightField(_map, x, maxRange))
                    .ToHashSet();
            var finalLos = Sighting.GetSightField(_map, path.Destination, maxRange).ToHashSet();
            var result = 
                Enumerable.Concat(Enumerable.Concat(initialLos, finalLos), stepLos)
                    .Select(x => x.Target)
                    .Distinct()
                    .ToList();

            foreach (var los in initialLos)
            {
                _spotters.Remove(los.Target, unit);
            }
            foreach (var los in stepLos)
            {
                if (!los.IsBlocked && los.Distance <= visualRange)
                {
                    _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(los.Target));
                }
            }
            foreach (var los in finalLos)
            {
                if (!los.IsBlocked && los.Distance <= visualRange)
                {
                    _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(los.Target));
                }
                _spotters.Add(los.Target, unit);
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
            var visualRange = GetVisualRange(unit);
            foreach (var los in Sighting.GetSightField(_map, position, GetMaxRange(unit)))
            {
                if (!los.IsBlocked && los.Distance <= visualRange)
                {
                    _discovery.Discover(Cubic.HexagonalOffset.Instance.Project(los.Target));
                }
                _spotters.Add(los.Target, unit);
                result.Add(los.Target);
            }
            return result;
        }

        public List<Vector3i> Remove(IAsset asset)
        {
            if (asset is not Unit unit)
            {
                return new();
            }

            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(_map, asset.Position, GetMaxRange(unit)))
            {
                if (_spotters.Remove(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            return result;
        }

        private static int GetMaxRange(Unit unit)
        {
            return (int)Enum.GetValues<UnitDetectionBand>()
                .Select(x => unit.Type.Capabilities.GetRange(CombatCondition.None, x).GetValue()).Max();
        }

        private static int GetVisualRange(Unit unit)
        {
            return (int)unit.Type.Capabilities.GetRange(CombatCondition.None, UnitDetectionBand.Visual).GetValue();
        }
    }
}

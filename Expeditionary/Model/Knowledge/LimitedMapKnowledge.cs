using Cardamom.Collections;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class LimitedMapKnowledge
    {
        private readonly Map _map;
        private readonly IMapDiscovery _discovery;
        private readonly MultiMap<Vector3i, Unit> _spotters = new();

        public LimitedMapKnowledge(Map map, IMapDiscovery discovery) 
        {
            _map = map;
            _discovery = discovery;
        }

        public SingleTileKnowledge Get(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            if (!_map.Contains(coord))
            {
                return new(IsDiscovered: false, IsVisible: false);
            }
            var condition = _map.Get(hex)!.GetConditions();
            lock (_spotters)
            {
                return new(
                    _discovery.IsDiscovered(hex),
                    _spotters[hex]
                        .Any(x => x.Type.Capabilities.GetRange(condition, UnitDetectionBand.Visual).GetValue()
                            >= Geometry.GetCubicDistance(x.Position!.Value, hex)
                            && x.Position != null 
                            && Sighting.IsValidLineOfSight(_map, x.Position.Value, hex)));
            }
        }

        public EnumMap<UnitDetectionBand, float>? GetDetection(Vector3i hex)
        {
            lock (_spotters)
            {
                var condition = _map.Get(hex)!.GetConditions();
                var spotters = _spotters[hex].ToList();
                if (!spotters.Any())
                {
                    return null;
                }

                var result = new EnumMap<UnitDetectionBand, float>();
                foreach (var spotter in _spotters[hex])
                {
                    if (spotter.Position == null)
                    {
                        continue;
                    }
                    var los = Sighting.GetLineOfSight(_map, spotter.Position.Value, hex);
                    foreach (var band in Enum.GetValues<UnitDetectionBand>())
                    {
                        result[band] =
                            Math.Max(result[band], SpottingCalculator.GetDetection(spotter, band, los, condition));
                    }
                }
                return result;
            }
        }

        public Map GetMap()
        {
            return _map;
        }

        public List<Vector3i> Move(
            Unit unit,
            IEnumerable<Sighting.LineOfSight> initial,
            IEnumerable<Sighting.LineOfSight> medial,
            IEnumerable<Sighting.LineOfSight> final)
        {
            var visualRange = GetVisualRange(unit);
            var result = new List<Vector3i>();
            foreach (var los in initial)
            {
                if (!los.IsBlocked && los.Distance <= visualRange)
                {
                    result.Add(los.Target);
                }
                lock (_spotters)
                {
                    _spotters.Remove(los.Target, unit);
                }
            }
            foreach (var los in medial)
            {
                if (!los.IsBlocked
                    && los.Distance <= visualRange 
                    && _discovery.Discover(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            foreach (var los in final)
            {
                if (!los.IsBlocked && los.Distance <= visualRange)
                {
                    _discovery.Discover(los.Target);
                    result.Add(los.Target);
                }
                lock (_spotters)
                {
                    _spotters.Add(los.Target, unit);
                }
            }
            return result;
        }

        public List<Vector3i> Place(Unit unit, IEnumerable<Sighting.LineOfSight> delta)
        {
            var result = new List<Vector3i>();
            var visualRange = GetVisualRange(unit);
            lock (_spotters)
            {
                foreach (var los in delta)
                {
                    if (!los.IsBlocked && los.Distance <= visualRange)
                    {
                        _discovery.Discover(los.Target);
                    }
                    _spotters.Add(los.Target, unit);
                    result.Add(los.Target);
                }
            }
            return result;
        }

        public List<Vector3i> Remove(Unit unit, IEnumerable<Sighting.LineOfSight> delta)
        {
            var result = new List<Vector3i>();
            lock (_spotters)
            {
                foreach (var los in delta)
                {
                    if (_spotters.Remove(los.Target, unit))
                    {
                        result.Add(los.Target);
                    }
                }
            }
            return result;
        }

        private static int GetVisualRange(Unit unit)
        {
            return (int)unit.Type.Capabilities.GetRange(CombatCondition.None, UnitDetectionBand.Visual).GetValue();
        }
    }
}

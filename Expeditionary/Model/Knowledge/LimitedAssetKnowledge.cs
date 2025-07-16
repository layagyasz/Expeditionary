using Cardamom.Collections;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class LimitedAssetKnowledge
    {
        private readonly Player _player;
        private readonly Dictionary<IAsset, SingleAssetKnowledge> _assets = new();

        public LimitedAssetKnowledge(Player player)
        {
            _player = player;
        }

        public SingleAssetKnowledge Get(IAsset asset)
        {
            if (IsPlayer(asset))
            {
                return asset.IsDestroyed ? new(false, null) : new(true, asset.Position);
            }
            return _assets.TryGetValue(asset, out var value) ? value : new();
        }

        public List<IAsset> AddSelf(
            LimitedMapKnowledge mapKnowledge,
            Unit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(AddLos(mapKnowledge, unit, delta, positions, isPermanent: true));
            }
            return result;
        }

        public List<IAsset> AddOther(LimitedMapKnowledge mapKnowledge, IAsset asset, Vector3i position)
        {
            var result = new List<IAsset>();
            var current = new SingleAssetKnowledge();

            var detection = mapKnowledge.GetDetection(position);
            var condition = mapKnowledge.GetMap().Get(position)!.GetConditions();
            var spotted = detection != null && SpottingCalculator.IsSpotted(detection, condition, asset);
            if (spotted)
            {
                current.IsVisible = true;
                current.LastSeen = position;
                result.Add(asset);
            }
            _assets.Add(asset, current);
            return result;
        }
        
        public List<IAsset> DestroySelf(
            LimitedMapKnowledge mapKnowledge,
            Unit unit,
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IAsset> positions)
        {
            return RemoveSelf(mapKnowledge, unit, delta, positions);
        }

        public List<IAsset> DestroyOther(IAsset asset)
        {
            var result = new List<IAsset>();
            var current = _assets[asset];
            if (current.IsVisible)
            {
                result.Add(asset);
                current.IsVisible = false;
                current.LastSeen = null;
                _assets[asset] = current;
            }
            return result;
        }

        public List<IAsset> RemoveSelf(
            LimitedMapKnowledge mapKnowledge, 
            Unit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(RemoveLos(mapKnowledge, delta, positions));
            }
            return result;
        }

        public List<IAsset> RemoveOther(IAsset asset)
        {
            var result = new List<IAsset>();
            var current = _assets[asset];
            if (current.IsVisible)
            {
                result.Add(asset);
            }
            _assets.Remove(asset);
            return result;
        }

        public List<IAsset> MoveOther(LimitedMapKnowledge mapKnowledge, IAsset asset, Pathing.Path path)
        {
            var current = _assets[asset];
            for (int i=path.Steps.Count - 1; i>=0; --i)
            {
                var hex = path.Steps[i];
                var condition = mapKnowledge.GetMap().Get(hex)!.GetConditions();
                var detection = mapKnowledge.GetDetection(hex);
                var spotted = detection != null && SpottingCalculator.IsSpotted(detection, condition, asset);
                if (spotted)
                {
                    if (i == path.Steps.Count - 1)
                    {
                        current.IsVisible = true;
                        current.LastSeen = hex;
                        _assets[asset] = current;
                        return new() { asset };
                    }
                    else
                    {
                        current.IsVisible = false;
                        current.LastSeen = path.Steps[i + 1];
                        _assets[asset] = current;
                        return new() { asset };
                    }
                }
            }
            current.IsVisible = false;
            current.LastSeen = null;
            _assets[asset] = current;
            return new();
        }

        public List<IAsset> MoveSelf(
            LimitedMapKnowledge mapKnowledge,
            Unit unit,
            IEnumerable<Sighting.LineOfSight> initial,
            IEnumerable<Sighting.LineOfSight> medial,
            IEnumerable<Sighting.LineOfSight> final, 
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(RemoveLos(mapKnowledge, initial, positions));
                result.AddRange(AddLos(mapKnowledge, unit, medial, positions, isPermanent: false));
                result.AddRange(AddLos(mapKnowledge, unit, final, positions, isPermanent: true));
            }
            return result.Distinct().ToList();
        }

        private List<IAsset> AddLos(
            LimitedMapKnowledge mapKnowledge, 
            Unit unit,
            IEnumerable<Sighting.LineOfSight> delta, 
            MultiMap<Vector3i, IAsset> positions,
            bool isPermanent)
        {
            var result = new List<IAsset>();
            foreach (var los in delta)
            {
                var hex = los.Target;
                var tile = mapKnowledge.GetMap().Get(hex);
                if (tile == null)
                {
                    continue;
                }
                var condition = tile.GetConditions();
                var detection = SpottingCalculator.GetDetection(unit, los, condition);
                foreach (var asset in positions[hex].Where(x => !IsPlayer(x)))
                {
                    var current = _assets[asset];
                    var spotted = SpottingCalculator.IsSpotted(detection, condition, asset);
                    if (!current.IsVisible && spotted)
                    {
                        current.IsVisible = isPermanent;
                        current.LastSeen = hex;
                        result.Add(asset);
                    }
                    _assets[asset] = current;
                }
            }
            return result;
        }

        private List<IAsset> RemoveLos(
            LimitedMapKnowledge mapKnowledge, 
            IEnumerable<Sighting.LineOfSight> delta, 
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>();
            foreach (var los in delta)
            {
                var hex = los.Target;
                var tile = mapKnowledge.GetMap().Get(hex);
                if (tile == null)
                {
                    continue;
                }
                var condition = tile.GetConditions();
                var detection = mapKnowledge.GetDetection(hex);
                foreach (var asset in positions[hex].Where(x => !IsPlayer(x)))
                {
                    var current = _assets[asset];
                    var spotted = detection != null && SpottingCalculator.IsSpotted(detection, condition, asset);
                    if (current.IsVisible && !spotted)
                    {
                        current.IsVisible = false;
                        current.LastSeen = hex;
                        result.Add(asset);
                    }
                    _assets[asset] = current;
                }
            }
            return result;
        }

        private bool IsPlayer(IAsset asset)
        {
            return asset is Unit unit && unit.Player == _player;
        }
    }
}

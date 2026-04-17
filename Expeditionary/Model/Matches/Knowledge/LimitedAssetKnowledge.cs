using Cardamom.Collections;
using Expeditionary.Model.Matches.Assets;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public class LimitedAssetKnowledge
    {
        private readonly MatchPlayer _player;
        private readonly Dictionary<IMatchAsset, SingleAssetKnowledge> _assets = new();

        public LimitedAssetKnowledge(MatchPlayer player)
        {
            _player = player;
        }

        public SingleAssetKnowledge Get(IMatchAsset asset)
        {
            if (IsPlayer(asset))
            {
                return asset.IsDestroyed ? new(false, null) : new(true, asset.Position);
            }
            return _assets.TryGetValue(asset, out var value) ? value : new();
        }

        public List<IMatchAsset> AddSelf(
            LimitedMapKnowledge mapKnowledge,
            MatchUnit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IMatchAsset> positions)
        {
            var result = new List<IMatchAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(AddLos(mapKnowledge, unit, delta, positions, isPermanent: true));
            }
            return result;
        }

        public List<IMatchAsset> AddOther(LimitedMapKnowledge mapKnowledge, IMatchAsset asset, Vector3i position)
        {
            var result = new List<IMatchAsset>();
            if (asset.IsPassenger)
            {
                return result;
            }
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
            _assets[asset] = current;
            return result;
        }
        
        public List<IMatchAsset> DestroySelf(
            LimitedMapKnowledge mapKnowledge,
            MatchUnit unit,
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IMatchAsset> positions)
        {
            return RemoveSelf(mapKnowledge, unit, delta, positions);
        }

        public List<IMatchAsset> DestroyOther(IMatchAsset asset)
        {
            var result = new List<IMatchAsset>();
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

        public List<IMatchAsset> RemoveSelf(
            LimitedMapKnowledge mapKnowledge, 
            MatchUnit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IMatchAsset> positions)
        {
            var result = new List<IMatchAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(RemoveLos(mapKnowledge, delta, positions));
            }
            return result;
        }

        public List<IMatchAsset> RemoveOther(IMatchAsset asset)
        {
            var result = new List<IMatchAsset>();
            var current = _assets[asset];
            if (current.IsVisible)
            {
                result.Add(asset);
            }
            _assets.Remove(asset);
            return result;
        }

        public List<IMatchAsset> MoveOther(LimitedMapKnowledge mapKnowledge, IMatchAsset asset, Pathing.Path path)
        {
            if (asset.IsPassenger)
            {
                return new();
            }
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

        public List<IMatchAsset> MoveSelf(
            LimitedMapKnowledge mapKnowledge,
            MatchUnit unit,
            IEnumerable<Sighting.LineOfSight> initial,
            IEnumerable<Sighting.LineOfSight> medial,
            IEnumerable<Sighting.LineOfSight> final, 
            MultiMap<Vector3i, IMatchAsset> positions)
        {
            var result = new List<IMatchAsset>() { unit };
            if (!unit.IsPassenger)
            {
                result.AddRange(RemoveLos(mapKnowledge, initial, positions));
                result.AddRange(AddLos(mapKnowledge, unit, medial, positions, isPermanent: false));
                result.AddRange(AddLos(mapKnowledge, unit, final, positions, isPermanent: true));
            }
            return result.Distinct().ToList();
        }

        private List<IMatchAsset> AddLos(
            LimitedMapKnowledge mapKnowledge, 
            MatchUnit unit,
            IEnumerable<Sighting.LineOfSight> delta, 
            MultiMap<Vector3i, IMatchAsset> positions,
            bool isPermanent)
        {
            var result = new List<IMatchAsset>();
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

        private List<IMatchAsset> RemoveLos(
            LimitedMapKnowledge mapKnowledge, 
            IEnumerable<Sighting.LineOfSight> delta, 
            MultiMap<Vector3i, IMatchAsset> positions)
        {
            var result = new List<IMatchAsset>();
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

        private bool IsPlayer(IMatchAsset asset)
        {
            return asset is MatchUnit unit && unit.Player == _player;
        }
    }
}

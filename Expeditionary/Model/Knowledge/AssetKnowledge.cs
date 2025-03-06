using Cardamom.Collections;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class AssetKnowledge
    {
        private readonly Player _player;
        private readonly Dictionary<IAsset, SingleAssetKnowledge> _assets = new();

        public AssetKnowledge(Player player)
        {
            _player = player;
        }

        public SingleAssetKnowledge Get(IAsset asset)
        {
            if (IsPlayer(asset))
            {
                return new() { IsVisible = true, LastSeen = asset.Position };
            }
            return _assets.TryGetValue(asset, out var value) ? value : new();
        }

        public List<IAsset> AddSelf(
            MapKnowledge mapKnowledge,
            Unit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            result.AddRange(AddLos(mapKnowledge, unit, delta, positions, isPermanent: true));
            return result;
        }

        public List<IAsset> AddOther(MapKnowledge mapKnowledge, IAsset asset, Vector3i position)
        {
            var current = new SingleAssetKnowledge();
            _assets.Add(asset, current);

            var detection = mapKnowledge.GetDetection(position);
            var condition = mapKnowledge.GetMap().Get(position)!.GetConditions();
            var spotted = detection != null && SpottingCalculator.IsSpotted(detection, condition, asset);
            if (spotted)
            {
                current.IsVisible = true;
                current.LastSeen = position;
                return new() { asset };
            }
            return new();
        }

        public List<IAsset> RemoveSelf(
            MapKnowledge mapKnowledge, 
            Unit unit, 
            IEnumerable<Sighting.LineOfSight> delta,
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            result.AddRange(RemoveLos(mapKnowledge, delta, positions));
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

        public List<IAsset> MoveOther(MapKnowledge mapKnowledge, IAsset asset, Pathing.Path path)
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
                        return new() { asset };
                    }
                    else
                    {
                        current.IsVisible = false;
                        current.LastSeen = path.Steps[i + 1];
                        return new() { asset };
                    }
                }
            }
            current.IsVisible = false;
            current.LastSeen = null;
            return new();
        }

        public List<IAsset> MoveSelf(
            MapKnowledge mapKnowledge,
            Unit unit,
            IEnumerable<Sighting.LineOfSight> initial,
            IEnumerable<Sighting.LineOfSight> medial,
            IEnumerable<Sighting.LineOfSight> final, 
            MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>() { unit };
            result.AddRange(RemoveLos(mapKnowledge, initial, positions));
            result.AddRange(AddLos(mapKnowledge, unit, medial, positions, isPermanent: false));
            result.AddRange(AddLos(mapKnowledge, unit, final, positions, isPermanent: true));
            return result.Distinct().ToList();
        }

        private List<IAsset> AddLos(
            MapKnowledge mapKnowledge, 
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
                }
            }
            return result;
        }

        private List<IAsset> RemoveLos(
            MapKnowledge mapKnowledge, IEnumerable<Sighting.LineOfSight> delta, MultiMap<Vector3i, IAsset> positions)
        {
            var result = new List<IAsset>();
            foreach (var los in delta)
            {
                var hex = los.Target;
                var condition = mapKnowledge.GetMap().Get(hex)!.GetConditions();
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

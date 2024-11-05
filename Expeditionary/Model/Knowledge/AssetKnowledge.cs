using Cardamom.Collections;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
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

        public List<IAsset> Add(MapKnowledge mapKnowledge, IAsset asset, Vector3i position)
        {
            if (IsPlayer(asset))
            {
                return new() { asset };
            }

            var current = new SingleAssetKnowledge();
            _assets.Add(asset, current);

            var detection = mapKnowledge.GetDetection(position);
            var condition = mapKnowledge.GetMap().GetTile(position)!.GetConditions();
            var spotted = detection != null && SpottingCalculator.IsSpotted(detection, condition, asset);
            if (spotted)
            {
                current.IsVisible = true;
                current.LastSeen = position;
                return new() { asset };
            }
            return new();
        }

        public List<IAsset> DoDelta(
            MapKnowledge mapKnowledge, MultiMap<Vector3i, IAsset> positions, IEnumerable<Vector3i> delta)
        {
            var result = new List<IAsset>();
            foreach (var hex in delta)
            {
                var condition = mapKnowledge.GetMap().GetTile(hex)!.GetConditions();
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
                    else if (!current.IsVisible && spotted)
                    {
                        current.IsVisible = true;
                        current.LastSeen = hex;
                        result.Add(asset);
                    }
                }
            }
            return result;
        }

        public List<IAsset> Remove(IAsset asset)
        {
            if (IsPlayer(asset))
            {
                return new() { asset };
            }

            var result = new List<IAsset>();
            var current = _assets[asset];
            if (current.IsVisible)
            {
                result.Add(asset);
            }
            _assets.Remove(asset);
            return result;
        }

        private bool IsPlayer(IAsset asset)
        {
            return asset is Unit unit && unit.Player == _player;
        }
    }
}

using Cardamom.Collections;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class PlayerKnowledge
    {
        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        private readonly Player _player;
        private readonly AssetKnowledge _assets;
        private readonly MapKnowledge _map;

        public PlayerKnowledge(Player player, AssetKnowledge assetKnowledge, MapKnowledge mapKnowledge)
        {
            _player = player;
            _assets = assetKnowledge;
            _map = mapKnowledge;
        }

        public void Add(IAsset asset, Vector3i position, MultiMap<Vector3i, IAsset> positions)
        {
            var mapDelta = new List<Vector3i>();
            var assetDelta = new List<IAsset>();
            if (asset is Unit unit && unit.Player == _player)
            {
                mapDelta.AddRange(_map.Place(asset, position));
                assetDelta.AddRange(_assets.DoDelta(_map, positions, mapDelta));
            }
            assetDelta.AddRange(_assets.Add(_map, asset, position));
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        public SingleAssetKnowledge GetAsset(IAsset asset)
        {
            return _assets.Get(asset);
        }

        public SingleTileKnowledge GetTile(Vector3i hex)
        {
            return _map.Get(hex);
        }

        public void Move(IAsset asset, Pathing.Path path, MultiMap<Vector3i, IAsset> positions)
        {
            // TODO -- Implement asset knowledge delta for enemy moves
            var mapDelta = new List<Vector3i>();
            var assetDelta = new List<IAsset>();
            if (asset is Unit unit && unit.Player == _player)
            {
                mapDelta.AddRange(_map.Move(asset, path));
                assetDelta.AddRange(_assets.DoDelta(_map, positions, mapDelta));
            }
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        public void Remove(IAsset asset, MultiMap<Vector3i, IAsset> positions)
        {
            var mapDelta = new List<Vector3i>();
            var assetDelta = new List<IAsset>();
            if (asset is Unit unit && unit.Player == _player)
            {
                mapDelta.AddRange(_map.Remove(asset));
                assetDelta.AddRange(_assets.DoDelta(_map, positions, mapDelta));
            }
            assetDelta.AddRange(_assets.Remove(asset));
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }
    }
}

using Cardamom.Collections;
using Expeditionary.Model.Matches.Assets;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public class OmniscientPlayerKnowledge : IPlayerKnowledge
    {
        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        private readonly Player _player;

        public OmniscientPlayerKnowledge(Player player)
        {
            _player = player;
        }

        public SingleAssetKnowledge GetAsset(IMatchAsset asset)
        {
            return asset.IsDestroyed ? new(false, null) : new(true, asset.Position);
        }

        public SingleTileKnowledge GetTile(Vector3i hex)
        {
            return new(true, true);
        }

        public void Destroy(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Move(IMatchAsset asset, Pathing.Path path, MultiMap<Vector3i, IMatchAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Place(IMatchAsset asset, Vector3i position, MultiMap<Vector3i, IMatchAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Remove(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Suppress(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }
    }
}

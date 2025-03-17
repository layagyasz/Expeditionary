using Cardamom.Collections;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
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

        public SingleAssetKnowledge GetAsset(IAsset asset)
        {
            return asset.IsDestroyed ? new(false, null) : new(true, asset.Position);
        }

        public SingleTileKnowledge GetTile(Vector3i hex)
        {
            return new(true, true);
        }

        public void Add(IAsset asset, Vector3i position, MultiMap<Vector3i, IAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Destroy(IAsset asset, MultiMap<Vector3i, IAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Move(IAsset asset, Pathing.Path path, MultiMap<Vector3i, IAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }

        public void Remove(IAsset asset, MultiMap<Vector3i, IAsset> positions)
        {
            AssetKnowledgeChanged?.Invoke(this, new(_player, new() { asset }));
        }
    }
}

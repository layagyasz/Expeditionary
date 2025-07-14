using Cardamom.Collections;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public interface IPlayerKnowledge
    {
        EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        SingleAssetKnowledge GetAsset(IAsset asset);
        SingleTileKnowledge GetTile(Vector3i hex);
        void Destroy(IAsset asset, MultiMap<Vector3i, IAsset> positions);
        void Move(IAsset asset, Pathing.Path path, MultiMap<Vector3i, IAsset> positions);
        void Place(IAsset asset, Vector3i position, MultiMap<Vector3i, IAsset> positions);

        void Remove(IAsset asset, MultiMap<Vector3i, IAsset> positions);
        void Suppress(IAsset asset, MultiMap<Vector3i, IAsset> positions);
    }
}

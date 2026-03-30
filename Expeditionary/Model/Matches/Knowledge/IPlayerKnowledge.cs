using Cardamom.Collections;
using Expeditionary.Model.Matches.Assets;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public interface IPlayerKnowledge
    {
        EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        SingleAssetKnowledge GetAsset(IMatchAsset asset);
        SingleTileKnowledge GetTile(Vector3i hex);
        void Destroy(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions);
        void Move(IMatchAsset asset, Pathing.Path path, MultiMap<Vector3i, IMatchAsset> positions);
        void Place(IMatchAsset asset, Vector3i position, MultiMap<Vector3i, IMatchAsset> positions);

        void Remove(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions);
        void Suppress(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions);
    }
}

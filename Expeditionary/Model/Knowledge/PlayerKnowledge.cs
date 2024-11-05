namespace Expeditionary.Model.Knowledge
{
    public record class PlayerKnowledge
    {
        public AssetKnowledge AssetKnowledge { get; init; }
        public MapKnowledge MapKnowledge { get; init; }

        public PlayerKnowledge(AssetKnowledge assetKnowledge, MapKnowledge mapKnowledge)
        {
            AssetKnowledge = assetKnowledge;
            MapKnowledge = mapKnowledge;
        }
    }
}

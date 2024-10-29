namespace Expeditionary.Model.Knowledge
{
    public record class PlayerKnowledge
    {
        public MapKnowledge MapKnowledge { get; init; }

        public PlayerKnowledge(MapKnowledge mapKnowledge)
        {
            MapKnowledge = mapKnowledge;
        }
    }
}

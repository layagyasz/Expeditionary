using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public record class MapKnowledgeChangedEventArgs
    {
        public Player Player { get; init; }
        public List<Vector3i> Delta { get; init; }

        public MapKnowledgeChangedEventArgs(Player player, List<Vector3i> delta)
        {
            Player = player;
            Delta = delta;
        }
    }
}

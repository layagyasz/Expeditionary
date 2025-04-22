using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public record class MapKnowledgeChangedEventArgs(Player Player, List<Vector3i> Delta)
    {
        public override string ToString()
        {
            return $"[MapKnowledgeChangedEventArgs: Player={Player}, Delta={string.Join(",", Delta)}]";
        }
    }
}

using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public record class MapKnowledgeChangedEventArgs(MatchPlayer Player, List<Vector3i> Delta)
    {
        public override string ToString()
        {
            return $"[MapKnowledgeChangedEventArgs: Player={Player}, Delta={string.Join(",", Delta)}]";
        }
    }
}

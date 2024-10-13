using Cardamom.Collections;

namespace Expeditionary.Model.Mapping
{
    public class Edge
    {
        public enum EdgeType
        {
            None,
            River,
            Road
        }

        public EnumMap<EdgeType, int> Levels { get; set; } = new();
    }
}

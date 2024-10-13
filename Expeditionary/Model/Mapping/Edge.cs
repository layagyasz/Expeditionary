using Cardamom.Collections;

namespace Expeditionary.Model.Mapping
{
    public class Edge
    {
        public EnumMap<EdgeType, int> Levels { get; set; } = new();
    }
}

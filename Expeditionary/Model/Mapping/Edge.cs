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

        public EdgeType Type { get; set; }
        public int Level { get; set; }
    }
}

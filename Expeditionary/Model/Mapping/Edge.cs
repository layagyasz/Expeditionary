namespace Expeditionary.Model.Mapping
{
    public class Edge
    {
        public enum EdgeType
        {
            None,
            River
        }

        public EdgeType Type { get; set; }
    }
}

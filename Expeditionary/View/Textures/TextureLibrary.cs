namespace Expeditionary.View.Textures
{
    public class TextureLibrary
    {
        public EdgeLibrary Edges { get; }
        public PartitionLibrary Partitions { get; }

        public TextureLibrary(PartitionLibrary partitions, EdgeLibrary edges)
        {
            Partitions = partitions;
            Edges = edges;
        }
    }
}

namespace Expeditionary.View.Textures
{
    public class TextureLibrary
    {
        public EdgeLibrary Edges { get; }
        public PartitionLibrary Partitions { get; }
        public StructureLibrary Structures { get; }

        public TextureLibrary(PartitionLibrary partitions, EdgeLibrary edges, StructureLibrary structures)
        {
            Partitions = partitions;
            Edges = edges;
            Structures = structures;
        }
    }
}

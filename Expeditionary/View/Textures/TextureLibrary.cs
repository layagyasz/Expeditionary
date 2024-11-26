namespace Expeditionary.View.Textures
{
    public class TextureLibrary
    {
        public EdgeLibrary Edges { get; }
        public MaskLibrary Masks { get; }
        public PartitionLibrary Partitions { get; }
        public StructureLibrary Structures { get; }

        public TextureLibrary(
            EdgeLibrary edges, MaskLibrary masks, PartitionLibrary partitions, StructureLibrary structures)
        {
            Edges = edges;
            Masks = masks;
            Partitions = partitions;
            Structures = structures;
        }
    }
}

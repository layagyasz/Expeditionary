namespace Expeditionary.View.Textures
{
    public class TextureLibrary
    {
        public EdgeLibrary Rivers { get; }
        public EdgeLibrary Ridges { get; }
        public MaskLibrary Masks { get; }
        public PartitionLibrary Partitions { get; }
        public StructureLibrary Structures { get; }

        public TextureLibrary(
            EdgeLibrary rivers,
            EdgeLibrary ridges,
            MaskLibrary masks, 
            PartitionLibrary partitions,
            StructureLibrary structures)
        {
            Rivers = rivers;
            Ridges = ridges;
            Masks = masks;
            Partitions = partitions;
            Structures = structures;
        }
    }
}

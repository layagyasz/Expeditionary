using Cardamom.Graphics;

namespace Expeditionary.View.Textures
{
    public record class TextureLibrary(
        Texture Blank,
        EdgeLibrary Rivers,
        EdgeLibrary Ridges,
        MaskLibrary Masks,
        PartitionLibrary Partitions,
        StructureLibrary Structures);
}

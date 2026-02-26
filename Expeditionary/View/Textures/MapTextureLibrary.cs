using Cardamom.Graphics;

namespace Expeditionary.View.Textures
{
    public record class MapTextureLibrary(
        Texture Blank,
        EdgeLibrary Rivers,
        EdgeLibrary Ridges,
        FoliageLibrary Foliage,
        PartitionLibrary Partitions,
        StructureLibrary Structures);
}

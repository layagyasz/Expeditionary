namespace Expeditionary.View.Textures
{
    public class TextureLibrary
    {
        public EdgeLibrary Edges { get; }
        public TerrainLibrary Terrain { get; }

        public TextureLibrary(EdgeLibrary edges, TerrainLibrary terrain)
        {
            Edges = edges;
            Terrain = terrain;
        }
    }
}

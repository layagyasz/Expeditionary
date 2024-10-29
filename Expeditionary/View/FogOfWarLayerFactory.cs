using Cardamom.Graphics;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;

namespace Expeditionary.View
{
    public class FogOfWarLayerFactory
    {
        private readonly RenderShader _shader;
        private readonly PartitionLibrary _partitions;

        public FogOfWarLayerFactory(RenderShader shader, PartitionLibrary partitions)
        {
            _shader = shader;
            _partitions = partitions;
        }

        public FogOfWarLayer Create(Map map)
        {
            return new(map.Size, new VertexBuffer<Vertex3>(PrimitiveType.Triangles), _shader, _partitions);
        }
    }
}

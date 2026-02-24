using Cardamom;
using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Expeditionary.View.Common.Buffers
{
    public class LayeredVertexBuffer : ManagedResource
    {
        public class Builder
        {
            private readonly List<MaskedVertex3[]> _layers = new();

            public Builder AddLayer(int size)
            {
                _layers.Add(new MaskedVertex3[size]);
                return this;
            }

            public void SetVertex(int layer, int index, MaskedVertex3 value)
            {
                _layers[layer][index] = value;
            }

            public LayeredVertexBuffer Build()
            {
                var layers = new Layer[_layers.Count];
                for (int i = 0; i < layers.Length; ++i)
                {
                    layers[i] = new(new(_layers[i], PrimitiveType.Triangles));
                }
                return new(layers);
            }
        }

        public class Layer : ManagedResource
        {
            private VertexBuffer<MaskedVertex3>? _vertices;

            internal Layer(VertexBuffer<MaskedVertex3> vertices)
            {
                _vertices = vertices;
            }

            protected override void DisposeImpl()
            {
                _vertices?.Dispose();
                _vertices = null;
            }

            public void Draw(IRenderTarget target, RenderResources resources)
            {
                target.Draw(_vertices!, 0, _vertices!.Length, resources);
            }
        }

        private readonly Layer?[] _layers;

        LayeredVertexBuffer(Layer[] layers)
        {
            _layers = layers;
        }

        protected override void DisposeImpl()
        {
            for (int i = 0; i < _layers.Length; ++i)
            {
                _layers[i]?.Dispose();
                _layers[i] = null;
            }
        }

        public IEnumerable<Layer> GetLayers()
        {
            return _layers!;
        }

        public Layer GetLayer(int id)
        {
            return _layers[id]!;
        }
    }
}

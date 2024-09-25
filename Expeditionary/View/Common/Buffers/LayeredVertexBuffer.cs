using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Common.Buffers
{
    public class LayeredVertexBuffer : GraphicsResource, IRenderable
    {
        public class Builder
        {
            private readonly List<Vertex3[]> _layers = new();

            private Func<int, RenderResources>? _resourcesFn;

            public Builder AddLayer(int size)
            {
                _layers.Add(new Vertex3[size]);
                return this;
            }

            public Builder SetRenderResources(Func<int, RenderResources> resourcesFn)
            {
                _resourcesFn = resourcesFn;
                return this;
            }

            public void SetVertex(int layer, int index, Vertex3 value)
            {
                _layers[layer][index] = value;
            }

            public LayeredVertexBuffer Build()
            {
                var layers = new Layer[_layers.Count];
                for (int i = 0; i < layers.Length; ++i)
                {
                    layers[i] = new(i, _resourcesFn!, new(_layers[i], PrimitiveType.Triangles));
                }
                return new(layers);
            }
        }

        private class Layer : GraphicsResource, IRenderable
        {
            private readonly int _id;
            private readonly Func<int, RenderResources> _resourcesFn;

            private VertexBuffer<Vertex3>? _vertices;

            internal Layer(int id, Func<int, RenderResources> resourcesFn, VertexBuffer<Vertex3> vertices)
            {
                _id = id;
                _resourcesFn = resourcesFn;
                _vertices = vertices;
            }

            protected override void DisposeImpl()
            {
                _vertices?.Dispose();
                _vertices = null;
            }

            public void Draw(IRenderTarget target, IUiContext context)
            {
                target.Draw(_vertices!, 0, _vertices!.Length, _resourcesFn(_id));
            }

            public void Initialize() { }

            public void ResizeContext(Vector3 size) { }

            public void Update(long delta) { }
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

        public void Draw(IRenderTarget target, IUiContext context)
        {
            foreach (var layer in _layers)
            {
                layer!.Draw(target, context);
            }
        }

        public void Draw(int layer, IRenderTarget target, IUiContext context)
        {
            _layers[layer]!.Draw(target, context);
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size) { }

        public void Update(long delta)
        {
            foreach (var layer in _layers)
            {
                layer!.Update(delta);
            }
        }
    }
}

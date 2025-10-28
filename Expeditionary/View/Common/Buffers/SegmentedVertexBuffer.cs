using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Common.Buffers
{
    public class SegmentedVertexBuffer<T> : ManagedResource, IRenderable where T : struct
    {
        private readonly int _blocks;
        private readonly int _blockSize;
        private readonly Func<RenderResources> _resourcesFn;

        private VertexBuffer<T>? _vertices;

        private int _index = 0;
        private readonly Queue<int> _freed = new();

        public SegmentedVertexBuffer(int blocks, int blockSize, Func<RenderResources> resourcesFn)
        {
            _blocks = blocks;
            _blockSize = blockSize;
            _vertices = new(PrimitiveType.Triangles);
            _resourcesFn = resourcesFn;
        }

        public void Clear()
        {
            _index = 0;
            _freed.Clear();
            Initialize();
        }

        public void Clear(int block)
        {
            Set(block, new T[_blockSize]);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_vertices!, 0, _vertices!.Length, _resourcesFn());
        }

        public void Free(int block)
        {
            Clear(block);
            _freed.Enqueue(block);
        }

        public void Initialize()
        {
            _vertices!.Buffer(new T[_blocks * _blockSize], 0, _blocks * _blockSize);
        }

        public int Reserve()
        {
            if (_freed.Count > 0)
            {
                return _freed.Dequeue();
            }
            return _index++;
        }

        public void ResizeContext(Vector3 context) { }

        public void Set(int block, T[] vertices)
        {
            Precondition.Check(vertices.Length >= _blockSize);
            _vertices!.Sub(vertices, block * _blockSize, vertices.Length);
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _vertices?.Dispose();
            _vertices = null;
        }
    }
}

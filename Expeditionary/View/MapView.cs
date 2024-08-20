using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapView : GraphicsResource, IRenderable
    {
        protected override void DisposeImpl() { }

        public void Draw(IRenderTarget target, IUiContext context) { }

        public void Initialize() { }

        public void ResizeContext(Vector3 size) { }

        public void Update(long delta) { }
    }
}

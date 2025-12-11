using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Expeditionary.Model.Galaxies;
using Expeditionary.View.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyViewFactory
    {
        private static readonly Vector3[] s_Corners =
        {
            new(0f, 0f, 0f),
            new(1f, 0f, 0f),
            new(1f, 0f, 1f),
            new(0f, 0f, 1f)
        };
        private static readonly float s_GridWidth = 0.1f;
        private static readonly float s_GridZ = 1f;
        private static readonly Vector2i s_LookupSize = new(512, 512);
        private static readonly int s_TransformLocation = 0;

        private readonly ComputeShader _lookupShader;
        private readonly RenderShader _shader;
        private readonly RenderShader _filterShader;

        public GalaxyViewFactory(ComputeShader lookupShader, RenderShader shader, RenderShader filterShader)
        {
            _lookupShader = lookupShader;
            _shader = shader;
            _filterShader = filterShader;
        }

        public GalaxyView Create(Galaxy galaxy)
        {
            var transform = 
                Matrix4.CreateScale(2f / s_LookupSize.X, 2f / s_LookupSize.Y, 1f)
                * Matrix4.CreateTranslation(-1, -1, 0);
            _lookupShader.SetMatrix4(s_TransformLocation, transform);

            var p = new Texture.Parameters() { WrapMode = TextureWrapMode.ClampToEdge };
            var tex = Texture.Create(s_LookupSize, p);
            tex.BindImage(0);
            _lookupShader.DoCompute(s_LookupSize);
            Texture.UnbindImage(0);

            ArrayList<Vertex3> sectors = new();
            foreach (var sector in galaxy.Sectors)
            {
                Shapes.AddVertices(
                    sectors,
                    sector.Color,
                    new Line3(
                        s_Corners.Select(x => x * ToVector3(sector.Size) + ToVector3(sector.TopLeft)).ToArray(),
                        new Vector3(0, 1, 0), 
                        isLoop: true),
                    s_GridWidth,
                    center: false);
            }

            return new(
                galaxy.Scale,
                tex,
                Texture.FromFile("resources/view/textures/luts/lut_galaxy.png", p),
                new VertexBuffer<Vertex3>(sectors.GetData(), PrimitiveType.Triangles), 
                _shader, 
                _filterShader);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, s_GridZ, x.Y);
        }
    }
}

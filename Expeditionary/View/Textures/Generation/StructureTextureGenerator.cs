using Cardamom.Graphics.TexturePacking;
using Cardamom.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Cardamom.Collections;
using Cardamom.Mathematics.Geometry;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Common;

namespace Expeditionary.View.Textures.Generation
{
    public class StructureTextureGenerator
    {
        private record struct Endpoint(int Face, int Anchor, int Angle);
        private record struct Arc(Endpoint Left, Endpoint Right);

        private static readonly int SheetSize = 2048;
        private static readonly int TexSize = 128;
        private static readonly float Sqrt3d2 = 0.5f * MathF.Sqrt(3);
        private static readonly float Radius = 64;
        private static readonly Vector3[] Corners =
        {
            new(-0.5f, -Sqrt3d2, 0f),
            new(0.5f, -Sqrt3d2, 0f),
            new(1f, 0f, 0f),
            new(0.5f, Sqrt3d2, 0f),
            new(-0.5f, Sqrt3d2, 0f),
            new(-1f, 0f, 0f)
        };
        private static readonly Vector3[] Angles =
        {
            new(0f, -1f, 0f),
            new(0.5f, -Sqrt3d2, 0f),
            new(Sqrt3d2, -0.5f, 0f),

            new(1f, 0f, 0f),
            new(Sqrt3d2, 0.5f, 0f),
            new(0.5f, Sqrt3d2, 0f),

            new(0f, 1f, 0f),
            new(-0.5f, Sqrt3d2, 0f),
            new(-Sqrt3d2, 0.5f, 0f),

            new(-1f, 0f, 0f),
            new(-Sqrt3d2, -0.5f, 0f),
            new(-0.5f, -Sqrt3d2, 0f)
        };
        private static readonly float[] Anchors =
        {
            0.125f,
            0.25f,
            0.5f,
            0.75f,
            0.875f
        };

        private static readonly Arc[][] Arcs =
        {
            Array.Empty<Arc>(),
            new Arc[] { new(new(0, 0, 0), new(0, 0, 0)) },
            new Arc[] { new(new(0, 0, 0), new(1, 0, 0)) },
            new Arc[] { new(new(0, 0, 0), new(2, 0, 0)) },
            new Arc[] { new(new(0, 0, 0), new(3, 0, 0)) },
            new Arc[] 
            { 
                new(new(0, 0, 0), new(1, 0, 0)),
                new(new(0, 0, 0), new(2, 0, 0)),
                new(new(1, 0, 0), new(2, 0, 0))
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(1, 0, 0)),
                new(new(0, 0, 0), new(3, 0, 0)),
                new(new(1, 0, 0), new(3, 0, 0))
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(2, 0, 0)),
                new(new(0, 0, 0), new(4, 0, 0)),
                new(new(2, 0, 0), new(4, 0, 0))
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(3, 0, 0)),
                new(new(0, 0, 0), new(1, 0, 0)),
                new(new(2, 0, 0), new(1, 0, 0)),
                new(new(2, 0, 0), new(3, 0, 0)),
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(3, 0, 0)),
                new(new(1, 0, 0), new(4, 0, 0)),
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(3, 0, 0)),
                new(new(0, 0, 0), new(2, 0, 0)),
                new(new(0, 0, 0), new(4, 0, 0)),
                new(new(2, 0, 0), new(4, 0, 0)),
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(2, 0, 0)),
                new(new(0, 0, 0), new(4, 0, 0)),
                new(new(1, 0, 0), new(5, 0, 0)),
            },
            new Arc[]
            {
                new(new(0, 0, 0), new(3, 0, 0)),
                new(new(1, 0, 0), new(4, 0, 0)),
                new(new(2, 0, 0), new(5, 0, 0)),
            },
        };

        private static readonly Color4 RoadColor = new(90, 90, 110, 255);
        private static readonly float RoadWidth = 1f / 16f;
        private static readonly int Points = 16;

        private readonly RenderShader _shader;

        public StructureTextureGenerator(RenderShader shader)
        {
            _shader = shader;
        }

        public StructureLibrary Generate()
        {
            var renderTexture = new RenderTexture(new(TexSize, TexSize));
            renderTexture.PushProjection(
                new(
                    -10, 
                    Matrix4.CreateOrthographicOffCenter(
                        -TexSize / 2, TexSize / 2, -TexSize / 2, TexSize /2, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            var sheet =
                new DynamicStaticSizeTexturePage(
                    new(SheetSize, SheetSize),
                    new(TexSize, TexSize),
                    new(),
                    new(),
                    new()
                    {
                        MinFilter = TextureMinFilter.Nearest,
                        MagFilter = TextureMagFilter.Nearest
                    });

            var options = new StructureLibrary.Option[Arcs.Length];
            for (int i=0; i<Arcs.Length; ++i)
            {
                var arcs = Arcs[i];
                var vertices = new ArrayList<Vertex3>();
                foreach (var arc in arcs)
                {
                    var left = GetAnchor(arc.Left.Face, arc.Left.Anchor);
                    var right = GetAnchor(arc.Right.Face, arc.Right.Anchor);
                    var tLeft = -GetAngle(arc.Left.Face, arc.Left.Angle);
                    var tRight = GetAngle(arc.Right.Face, arc.Right.Angle);
                    var line = 
                        new Line3(
                            Enumerable.Range(0, Points)
                                .Select(x => Spline.GetPoint(1f * x / (Points - 1), left, right, tLeft, tRight))
                                .ToArray(),
                            Vector3.UnitZ);
                    Shapes.AddVertices(vertices, RoadColor, line, Radius * RoadWidth, center: true);
                }

                renderTexture.Clear();
                
                /*
                for (int j = 0; j < Corners.Length; ++j)
                {
                    var verts = new Vertex3[3];
                    verts[0] = new(new(), Color4.White, new());
                    verts[1] = new(Radius * Corners[j], Color4.White, new());
                    verts[2] = new(Radius * Corners[(j + 1) % Corners.Length], Color4.White, new());
                    renderTexture.Draw(verts, PrimitiveType.Triangles, 0, 3, new(BlendMode.None, _shader));
                }
                */
                
                renderTexture.Draw(
                    vertices.GetData(), PrimitiveType.Triangles, 0, vertices.Count, new(BlendMode.None, _shader));
                renderTexture.Display();
                // renderTexture.GetTexture().CopyToImage().SaveToFile($"road-0-{i}.png");
                sheet.Add(renderTexture.GetTexture(), out var segment);

                options[i] = MakeOption(arcs, segment);
            }
            // sheet.GetTexture().CopyToImage().SaveToFile("roads2.png");
            return new StructureLibrary(sheet.GetTexture(), options);
        }

        private static Vector3 GetAnchor(int face, int anchor)
        {
            var left = Corners[face];
            var right = Corners[(face + 1) % 6];
            var a = Anchors[anchor + 2];
            return 1.05f * Radius * ((1 - a) * left + a * right);
        }

        private static Vector3 GetAngle(int face, int angle)
        {
            return 128f * Angles[(2 * face + angle + 12) % 12];
        }

        private static StructureLibrary.Option MakeOption(Arc[] arcs, Box2i segment)
        {
            var connections = new StructureLibrary.Connection[6];
            for (int i=0; i<6; ++i)
            {
                connections[i] = 
                    new StructureLibrary.Connection(
                        new StructureLibrary.ConnectionType[5], new int[5], new int[5]);
            }
            foreach (var arc in arcs)
            {
                AddEndpoint(connections, arc.Left);
                AddEndpoint(connections, arc.Right);
            }
            return new StructureLibrary.Option(
                segment.Center,
                Corners.Select(x => Radius * x.Xy + segment.Center).ToArray(), 
                new(StructureType.None, 0, connections));
        }

        private static void AddEndpoint(StructureLibrary.Connection[] connections, Endpoint endpoint)
        {
            var connection = connections[endpoint.Face];
            connection.Type[endpoint.Anchor] = StructureLibrary.ConnectionType.Road;
            connection.Level[endpoint.Anchor] = 1;
            connection.Angle[endpoint.Anchor] = endpoint.Anchor;
        }
    }
}

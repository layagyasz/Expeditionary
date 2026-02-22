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

        private static readonly float s_Sqrt3d2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] s_Corners =
        {
            new(-0.5f, -s_Sqrt3d2, 0f),
            new(0.5f, -s_Sqrt3d2, 0f),
            new(1f, 0f, 0f),
            new(0.5f, s_Sqrt3d2, 0f),
            new(-0.5f, s_Sqrt3d2, 0f),
            new(-1f, 0f, 0f)
        };
        private static readonly Vector3[] s_Angles =
        {
            new(0f, -1f, 0f),
            new(0.5f, -s_Sqrt3d2, 0f),
            new(s_Sqrt3d2, -0.5f, 0f),

            new(1f, 0f, 0f),
            new(s_Sqrt3d2, 0.5f, 0f),
            new(0.5f, s_Sqrt3d2, 0f),

            new(0f, 1f, 0f),
            new(-0.5f, s_Sqrt3d2, 0f),
            new(-s_Sqrt3d2, 0.5f, 0f),

            new(-1f, 0f, 0f),
            new(-s_Sqrt3d2, -0.5f, 0f),
            new(-0.5f, -s_Sqrt3d2, 0f)
        };
        private static readonly float[] s_Anchors =
        {
            0.125f,
            0.25f,
            0.5f,
            0.75f,
            0.875f
        };

        private static readonly Arc[][] s_Arcs =
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

        private static readonly Color4 s_RoadColor = new(90, 90, 110, 255);
        private static readonly float s_RoadWidth = 4f;
        private static readonly int s_Points = 16;

        private readonly RenderShader _shader;

        public StructureTextureGenerator(RenderShader shader)
        {
            _shader = shader;
        }

        public StructureLibrary Generate()
        {
            var renderTexture = new RenderTexture(new(128, 128));
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(-64, 64, -64, 64, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            var sheet =
                new DynamicStaticSizeTexturePage(
                    new(2048, 2048),
                    new(128, 128),
                    new(),
                    new(),
                    new()
                    {
                        MinFilter = TextureMinFilter.Nearest,
                        MagFilter = TextureMagFilter.Nearest
                    });

            var options = new StructureLibrary.Option[s_Arcs.Length];
            for (int i=0; i<s_Arcs.Length; ++i)
            {
                var arcs = s_Arcs[i];
                var vertices = new ArrayList<Vertex3>();
                foreach (var arc in arcs)
                {
                    var left = GetAnchor(arc.Left.Face, arc.Left.Anchor);
                    var right = GetAnchor(arc.Right.Face, arc.Right.Anchor);
                    var tLeft = -GetAngle(arc.Left.Face, arc.Left.Angle);
                    var tRight = GetAngle(arc.Right.Face, arc.Right.Angle);
                    var line = 
                        new Line3(
                            Enumerable.Range(0, s_Points)
                                .Select(x => Spline.GetPoint(1f * x / (s_Points - 1), left, right, tLeft, tRight))
                                .ToArray(),
                            Vector3.UnitZ);
                    Shapes.AddVertices(vertices, s_RoadColor, line, s_RoadWidth, center: true);
                }

                renderTexture.Clear();
                renderTexture.Draw(
                    vertices.GetData(), PrimitiveType.Triangles, 0, vertices.Count, new(BlendMode.None, _shader));
                renderTexture.Display();
                sheet.Add(renderTexture.GetTexture(), out var segment);

                options[i] = MakeOption(arcs, segment);
            }

            return new StructureLibrary(sheet.GetTexture(), options);
        }

        private static Vector3 GetAnchor(int face, int anchor)
        {
            var left = s_Corners[face];
            var right = s_Corners[(face + 1) % 6];
            var a = s_Anchors[anchor + 2];
            return 1.05f * 64f * ((1 - a) * left + a * right);
        }

        private static Vector3 GetAngle(int face, int angle)
        {
            return 128f * s_Angles[(2 * face + angle + 12) % 12];
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
                s_Corners.Select(x => 64f * x.Xy + segment.Center).ToArray(), 
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

using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.Model.Units;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures.Generation.Combat.Units
{
    public class UnitTextureGenerator
    {
        private static readonly string s_BackgroundKey = "icon_unit_background";
        private static Vector3 s_SizeOffset = new(0, -0.5f, 0);
        private static Vector3 s_Scale = new(0.6f, 0.4f, 1f);
        private static readonly Vector3[] s_Vertices =
            new Vector3[]
            {
                s_Scale * new Vector3(-1f, -1f, 0f),
                s_Scale * new Vector3(1f, -1f, 0f),
                s_Scale * new Vector3(-1f, 1f, 0f),

                s_Scale * new Vector3(-1f, 1f, 0f),
                s_Scale * new Vector3(1f, -1f, 0f),
                s_Scale * new Vector3(1f, 1f, 0f),
            };

        private static readonly float s_TextScale = 0.01f;
        private static readonly uint s_TextSize = 12;
        private static readonly Vector3 s_TextPin = new(0, 0.6f, 0);

        private readonly UnitTextureGeneratorSettings _settings;

        public UnitTextureGenerator(UnitTextureGeneratorSettings settings)
        {
            _settings = settings;
        }

        public ITextureVolume Generate(IEnumerable<UnitType> units)
        {
            var volume =
                new DynamicTextureVolume(
                    new DynamicStaticSizeTexturePage.Supplier(
                        _settings.TextureSize, _settings.ElementSize, new(), new(), new()),
                    checkAllPages: false);
            using (var renderTexture = new RenderTexture(_settings.ElementSize))
            {
                renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -10, 10)));
                renderTexture.PushViewMatrix(Matrix4.Identity);
                renderTexture.PushModelMatrix(Matrix4.Identity);
                using var text = new Text();
                text.SetFont(_settings.Font!);
                text.SetColor(Color4.White);
                text.SetCharacterSize(s_TextSize);
                text.SetShader(_settings.Shader!);

                RenderBackground(renderTexture);
                volume.Add(s_BackgroundKey, renderTexture.GetTexture());
                foreach (var unit in units)
                {
                    RenderUnit(unit, text, renderTexture);
                    volume.Add(unit.Definition.Key, renderTexture.GetTexture());
                }
            }

            return volume;
        }

        private void RenderBackground(IRenderTarget target)
        {
            var vertices = new ArrayList<Vertex3>();
            AddSegment(vertices, _settings.Images!.Get(_settings.BackgroundImage), Color4.White, new());
            target.Clear();
            target.Draw(
                vertices.GetData(),
                PrimitiveType.Triangles, 
                0, 
                vertices.Count, 
                new(BlendMode.Alpha, _settings.Shader!, _settings.Images.GetTextures().First()));
            target.Display();
        }

        private void RenderUnit(UnitType unit, Text text, IRenderTarget target) 
        {
            var vertices = new ArrayList<Vertex3>();
            AddSegment(vertices, _settings.Images!.Get(_settings.BorderImage), Color4.White, new());
            foreach (var tag in unit.GetTags())
            {
                if (_settings.TagImages.TryGetValue(tag, out var textureId))
                {
                    AddSegment(
                        vertices,
                        _settings.Images!.Get(textureId),
                        Color4.White,
                        IsSize(tag) ? s_SizeOffset : Vector3.Zero);
                }
            }
            target.Clear();
            target.Draw(
                vertices.GetData(), 
                PrimitiveType.Triangles,
                0, 
                vertices.Count,
                new(BlendMode.Alpha, _settings.Shader!, _settings.Images.GetTextures().First()));

            text.SetText(unit.Definition.Symbol ?? unit.Definition.Name);
            var size = 0.01f * text.Size;

            target.PushModelMatrix(
                Matrix4.CreateScale(s_TextScale) * 
                Matrix4.CreateTranslation(new(s_TextPin.X - 0.5f * size.X, s_TextPin.Y - size.Y, 0)));
            text.Draw(target, new SimpleUiContext());
            target.PopModelMatrix();

            target.Display();
        }

        private static void AddSegment(
            ArrayList<Vertex3> vertices, TextureSegment segment, Color4 color, Vector3 offset)
        {
            vertices.Add(new(offset + s_Vertices[0], color, segment.TextureView.Min));
            vertices.Add(
                new(offset + s_Vertices[1], color, new(segment.TextureView.Max.X, segment.TextureView.Min.Y)));
            vertices.Add(
                new(offset + s_Vertices[2], color, new(segment.TextureView.Min.X, segment.TextureView.Max.Y)));
            vertices.Add(
                new(offset + s_Vertices[3], color, new(segment.TextureView.Min.X, segment.TextureView.Max.Y)));
            vertices.Add(
                new(offset + s_Vertices[4], color, new(segment.TextureView.Max.X, segment.TextureView.Min.Y)));
            vertices.Add(new(offset + s_Vertices[5], color, segment.TextureView.Max));
        }

        private static bool IsSize(UnitTag tag)
        {
            return tag == UnitTag.Platoon || tag == UnitTag.Section;
        }
    }
}

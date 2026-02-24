using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Expeditionary.View
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct MaskedVertex3
    {
        [VertexAttribute(0, 3, VertexAttribPointerType.Float, false)]
        public Vector3 Position;

        [VertexAttribute(1, 4, VertexAttribPointerType.Float, false)]
        public Color4 Color;

        [VertexAttribute(2, 2, VertexAttribPointerType.Float, false)]
        public Vector2 TexCoords;

        [VertexAttribute(3, 2, VertexAttribPointerType.Float, false)]
        public Vector2 MaskTexCoords;

        public MaskedVertex3(Vector3 position, Color4 color, Vector2 texCoords, Vector2 maskTexCoords)
        {
            Position = position;
            Color = color;
            TexCoords = texCoords;
            MaskTexCoords = maskTexCoords;
        }
    }
}

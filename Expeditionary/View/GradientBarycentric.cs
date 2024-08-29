using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public struct GradientBarycentric
    {
        public Color4 A { get; set; }
        public Color4 B { get;set;}
        public Color4 C { get; set; }

        public Color4 Interpolate(Vector3 v)
        {
            return (Color4)(v.X * (Vector4)A + v.Y * (Vector4)B + v.Z * (Vector4)C);
        }
    }
}

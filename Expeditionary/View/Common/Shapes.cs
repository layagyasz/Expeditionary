using static Cardamom.Mathematics.Extensions;

using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;

namespace Expeditionary.View.Common
{
    public static class Shapes
    {
        private static readonly float Epsilon = 1e-07f;

        public struct WideSegment
        {
            public Vector3 NearLeft { get; set; }
            public Vector3 NearRight { get; set; }
            public Vector3 FarLeft { get; set; }
            public Vector3 FarRight { get; set; }

            public WideSegment(Vector3 nearLeft, Vector3 nearRight, Vector3 farLeft, Vector3 farRight)
            {
                NearLeft = nearLeft;
                NearRight = nearRight;
                FarLeft = farLeft;
                FarRight = farRight;
            }
        }

        public static void AddVertices(ArrayList<Vertex3> vertices, WideSegment segment, Color4 color)
        {
            vertices.Add(new(segment.NearLeft, color, new(0, 0)));
            vertices.Add(new(segment.NearRight, color, new(1, 0)));
            vertices.Add(new(segment.FarRight, color, new(1, 1)));
            vertices.Add(new(segment.FarRight, color, new(1, 1)));
            vertices.Add(new(segment.NearLeft, color, new(0, 0)));
            vertices.Add(new(segment.FarLeft, color, new(0, 1)));
        }

        public static void AddVertices(
            ArrayList<Vertex3> vertices, Color4 color, Line3 line, float width, bool center)
        {
            for (int i = 0; i < line.Count - (line.IsLoop ? 0 : 1); ++i)
            {
                var segment = line.GetSegment(i);
                Vector3 left;
                Vector3 right;
                if (line.IsLoop || i > 0)
                {
                    var l = line.GetSegment(i - 1);
                    var dl = (l.Left - l.Right).Normalized();
                    var d = (segment.Right - segment.Left).Normalized();
                    left = dl + d;
                    if (IsZero(left))
                    {
                        left = -Vector3.Cross(line.GetNormal(i), d);
                    }
                    else if (!HasSameDirection(Vector3.Cross(dl, d), line.GetNormal(i)))
                    {
                        left *= -1;
                    }
                }
                else
                {
                    left = Vector3.Cross(segment.Right - segment.Left, line.GetNormal(i));
                }
                if (line.IsLoop || i < line.Count - 2)
                {
                    var r = line.GetSegment(i + 1);
                    var dr = (r.Right - r.Left).Normalized();
                    var d = (segment.Left - segment.Right).Normalized();
                    right = dr + d;
                    if (IsZero(right))
                    {
                        right = Vector3.Cross(line.GetNormal(i), d);
                    }
                    else if (!HasSameDirection(Vector3.Cross(d, dr), line.GetNormal(i + 1)))
                    {
                        right *= -1;
                    }
                }
                else
                {
                    right = Vector3.Cross(segment.Right - segment.Left, line.GetNormal(i + 1));
                }
                AddVertices(vertices, CreateSegment(segment.Left, segment.Right, left, right, width, center), color);
            }
        }

        public static WideSegment CreateSegment(
            Ray3 ray, float length, Vector3 nearDirection, Vector3 farDirection, float width, bool center)
        {
            var far = ray.Get(length);
            nearDirection = width * nearDirection / Rejection(nearDirection, ray.Direction).Length;
            farDirection = width * farDirection / Rejection(farDirection, ray.Direction).Length;
            if (center)
            {
                return new(
                    ray.Point - 0.5f * nearDirection, ray.Point + 0.5f * nearDirection,
                    far - 0.5f * farDirection, far + 0.5f * farDirection);
            }
            else return new(ray.Point, ray.Point + nearDirection, far, far + farDirection);
        }

        public static WideSegment CreateSegment(
            Vector3 near, Vector3 far, Vector3 nearDirection, Vector3 farDirection, float width, bool center)
        {
            var d = far - near;
            float l = d.Length;
            d.Normalize();
            return CreateSegment(new(near, d), l, nearDirection, farDirection, width, center);
        }

        private static bool HasSameDirection(Vector3 left, Vector3 right)
        {
            return Vector3.Dot(left, right) > 0;
        }

        private static bool IsZero(Vector3 v)
        {
            return Math.Abs(v.X) < Epsilon && Math.Abs(v.Y) < Epsilon && Math.Abs(v.Z) < Epsilon;
        }
    }
}

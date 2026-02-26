using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.Graphics.TexturePacking;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace Expeditionary.View.Textures
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(Builder))]
    public class StructureLibrary
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ConnectionType
        {
            None,
            Road
        }

        public interface IConnectionQuery
        {
            bool Matches(Connection connection);
        }

        public record class OpenConnectionQuery(IList<(ConnectionType, int)> TypesAndLevels) : IConnectionQuery
        {
            public bool Matches(Connection connection)
            {
                foreach (var (type, level) in TypesAndLevels)
                {
                    if (!OtherContains(connection, type, level))
                    {
                        return false;
                    }
                }
                for (int i = 0; i < 5; ++i)
                {
                    if (!ThisContains(connection.Type[i], connection.Level[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            private static bool OtherContains(Connection connection, ConnectionType type, int level)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (type == connection.Type[i] && level == connection.Level[i])
                    {
                        return true;
                    }
                }
                return false;
            }

            private bool ThisContains(ConnectionType type, int level)
            {
                if (type == ConnectionType.None || level == 0)
                {
                    return true;
                }
                foreach (var (t, l) in TypesAndLevels)
                {
                    if (t == type && l == level)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public record class Connection(ConnectionType[] Type, int[] Level, int[] Angle) : IConnectionQuery
        {
            public bool Matches(Connection connection)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Type[i] != connection.Type[i])
                    {
                        return false;
                    }
                    if (Level[i] != connection.Level[i])
                    {
                        return false;
                    }
                    if (Angle[i] != connection.Angle[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public record struct Option(Vector2 TexCenter, Vector2[] TexCoords, StructureParameters Parameters);

        public record class StructureParameters(StructureType Type, int Level, Connection[] Connections);

        public record class StructureQuery(StructureType Type, int Level, IConnectionQuery[] Connections);

        public class Builder
        {
            public record class OptionBuilder {
                [JsonConverter(typeof(ReferenceJsonConverter))]
                public TextureSegment? Texture { get; set; }

                public StructureParameters? Parameters { get; set; }
            }

            [JsonConverter(typeof(FromMultipleFileJsonConverter))]
            public List<OptionBuilder> Options { get; set; } = new();

            [JsonConverter(typeof(TextureVolumeJsonConverter))]
            public ITextureVolume? Textures { get; set; }

            public StructureLibrary Build()
            {
                return new(
                    Textures!.GetSegments().Select(segment => segment.Texture!).Distinct().Single(),
                    Options.Select(
                        option => 
                            new Option(
                                option.Texture!.TextureView.Center,
                                GetTexCoords(option.Texture!.TextureView),
                                option.Parameters!))
                        .ToArray());
            }
        }

        public Texture Texture { get; }

        public Option[] Options { get; }

        public StructureLibrary(Texture texture, Option[] options)
        {
            Texture = texture;
            Options = options;
        }

        // TODO -- Add wildcard queries
        public IEnumerable<Option> Query(StructureQuery query)
        {
            return Options.SelectMany(AllTransforms).Where(option => Satisfies(query, option.Parameters));
        }

        private static IEnumerable<Option> AllTransforms(Option option)
        {
            for (int rotation = 0; rotation < 6; ++rotation)
            {
                yield return Transform(option, rotation, reflection: false);
                yield return Transform(option, rotation, reflection: true);
            }
        }

        private static Option Transform(Option option, int rotation, bool reflection)
        {
            if (reflection)
            {
                option = Reflect(option);
            }
            return new(
                option.TexCenter,
                Utils.Rotate(option.TexCoords, rotation), 
                new(
                    option.Parameters.Type,
                    option.Parameters.Level, 
                    Utils.Rotate(option.Parameters.Connections, rotation)));
        }

        private static readonly int[] s_CornerReflection = { 1, 0, 5, 4, 3, 2 };
        private static readonly int[] s_FaceReflection = { 0, 5, 4, 3, 2, 1 };
        private static Option Reflect(Option option)
        {
            var newOption =
                new Option(
                    option.TexCenter, 
                    Utils.Transform(option.TexCoords, s_CornerReflection),
                    new(
                        option.Parameters.Type,
                        option.Parameters.Level,
                        Utils.Transform(option.Parameters.Connections, s_FaceReflection)));
            for (int i=0; i<newOption.Parameters.Connections.Length; ++i)
            {
                newOption.Parameters.Connections[i] = 
                    new Connection(
                        newOption.Parameters.Connections[i].Type.Reverse().ToArray(),
                        newOption.Parameters.Connections[i].Level.Reverse().ToArray(),
                        newOption.Parameters.Connections[i].Angle.Reverse().Select(x => -x).ToArray());
            }
            return newOption;
        }

        private static bool Satisfies(StructureQuery query, StructureParameters parameters)
        {
            if (query.Type != parameters.Type)
            {
                return false;
            }
            if (query.Level != parameters.Level)
            {
                return false;
            }
            for (int i=0; i<6; ++i)
            {
                if (!query.Connections[i].Matches(parameters.Connections[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static readonly float Sqrt3d2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] Corners =
        {
            new(-0.5f, -Sqrt3d2, 0f),
            new(0.5f, -Sqrt3d2, 0f),
            new(1f, 0f, 0f),
            new(0.5f, Sqrt3d2, 0f),
            new(-0.5f, Sqrt3d2, 0f),
            new(-1f, 0f, 0f)
        };
        private static Vector2[] GetTexCoords(Box2i box)
        {
            var radius = 0.5f * Math.Min(box.Size.X, box.Size.Y);
            return Corners.Select(x => radius * x.Xy + box.Center).ToArray();
        }
    }
}

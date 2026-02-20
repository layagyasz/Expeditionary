using Cardamom.Graphics;
using Cardamom.Json.Graphics;
using Cardamom.Json.OpenTK;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace Expeditionary.View.Textures
{
    public class StructureLibrary
    {
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

        public record class Option(
            Vector2 TexCenter, Vector2[] TexCoords, StructureType Type, int Level, Connection[] Connections);

        public record class StructureQuery(StructureType Type, int Level, IConnectionQuery[] Connections);

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
            return Options.SelectMany(AllTransforms).Where(x => Satisfies(query, x));
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
                option.Type,
                option.Level, 
                Utils.Rotate(option.Connections, rotation));
        }

        private static readonly int[] s_CornerReflection = { 1, 0, 5, 4, 3, 2 };
        private static readonly int[] s_FaceReflection = { 0, 5, 4, 3, 2, 1 };
        private static Option Reflect(Option option)
        {
            var newOption =
                new Option(
                    option.TexCenter, 
                    Utils.Transform(option.TexCoords, s_CornerReflection),
                    option.Type,
                    option.Level,
                    Utils.Transform(option.Connections, s_FaceReflection));
            for (int i=0; i<newOption.Connections.Length; ++i)
            {
                newOption.Connections[i] = 
                    new Connection(
                        newOption.Connections[i].Type.Reverse().ToArray(),
                        newOption.Connections[i].Level.Reverse().ToArray(),
                        newOption.Connections[i].Angle.Reverse().Select(x => -x).ToArray());
            }
            return newOption;
        }

        private static bool Satisfies(StructureQuery query, Option option)
        {
            if (query.Type != option.Type)
            {
                return false;
            }
            if (query.Level != option.Level)
            {
                return false;
            }
            for (int i=0; i<6; ++i)
            {
                if (!query.Connections[i].Matches(option.Connections[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

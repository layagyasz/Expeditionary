using Cardamom.Graphics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures
{
    public class PartitionLibrary
    {
        private readonly static int[][] s_Transforms =
        {
            new int[] { 0, 1, 2 },
            new int[] { 2, 0, 1 },
            new int[] { 1, 2, 0 },
            new int[] { 0, 2, 1 },
            new int[] { 2, 1, 0 },
            new int[] { 1, 0, 2 }
        };

        public record class Option(Vector2[][] TexCoords);

        public Texture Texture { get; }
        public Option[] Options { get; }

        public PartitionLibrary(Texture texture, Option[] options)
        {
            Texture = texture;
            Options = options;
        }

        public IEnumerable<Option> Query()
        {
            foreach (var option in Options)
            {
                for (int i = 0; i < s_Transforms.Length; ++i)
                {
                    yield return new(
                        Transform(
                            option.TexCoords.Select(x => Transform(x, s_Transforms[i])).ToArray(), s_Transforms[i]));
                }
            }
        }

        private static T[] Transform<T>(T[] array, int[] transform)
        {
            var result = new T[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                result[i] = array[transform[i]];
            }
            return result;
        }
    }
}

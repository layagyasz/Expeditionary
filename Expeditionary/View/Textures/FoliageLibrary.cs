using Cardamom.Graphics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures
{
    public class FoliageLibrary
    {
        public record class Option(Vector2[] TexCoords, Vector3i Levels);

        public Texture Texture { get; }
        public Option[] Options { get; }

        public FoliageLibrary(Texture texture, Option[] options)
        {
            Texture = texture;
            Options = options;
        }

        public IEnumerable<Option> Query(Vector3i levels)
        {
            return Options.SelectMany(AllTransforms).Where(x => (x.Levels - levels).ManhattanLength == 0);
        }

        private readonly static int[][] s_Transforms =
        {
            new int[] { 0, 1, 2 },
            new int[] { 2, 0, 1 },
            new int[] { 1, 2, 0 },
            new int[] { 0, 2, 1 },
            new int[] { 2, 1, 0 },
            new int[] { 1, 0, 2 }
        };
        private static IEnumerable<Option> AllTransforms(Option option)
        {
            foreach (var transform in s_Transforms)
            {
                yield return new(Transform(option.TexCoords, transform), Transform(option.Levels, transform));
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

        private static Vector3i Transform(Vector3i levels, int[] transform)
        {
            return new(levels[transform[0]], levels[transform[1]], levels[transform[2]]);
        }
    }
}

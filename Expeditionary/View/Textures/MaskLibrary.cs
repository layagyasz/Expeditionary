using Cardamom.Graphics.TexturePacking;
using Cardamom.Graphics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures
{
    public class MaskLibrary
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

        public record class Option(Vector2[] TexCoords, Vector3i Levels);

        private readonly ITexturePage _texture;
        private readonly Option[] _options;

        private MaskLibrary(ITexturePage texture, Option[] options)
        {
            _texture = texture;
            _options = options;
        }

        public static MaskLibrary Create(ITexturePage texture, Option[] options)
        {
            var transformed = new Option[6 * options.Length];
            int i = 0;
            foreach (var option in options)
            {
                foreach (var transform in s_Transforms)
                {
                    transformed[i++] = 
                        new(Transform(option.TexCoords, transform), Transform(option.Levels, transform));
                }
            }
            return new(texture, transformed);
        }

        public Texture GetTexture()
        {
            return _texture.GetTexture();
        }

        public IEnumerable<Option> Query(Vector3i levels)
        {
            return _options.Where(x => (x.Levels - levels).ManhattanLength == 0);
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

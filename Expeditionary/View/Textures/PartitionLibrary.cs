using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
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

        private readonly ITexturePage _texture;
        private readonly Option[] _options;

        public PartitionLibrary(ITexturePage texture, Option[] options)
        {
            _texture = texture;
            _options = options;
        }

        public Texture GetTexture()
        {
            return _texture.GetTexture();
        }

        public IEnumerable<Option> Query()
        {
            foreach (var option in _options)
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

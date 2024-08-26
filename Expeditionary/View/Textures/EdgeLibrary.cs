using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures
{
    public class EdgeLibrary
    {
        private readonly static int[][] s_CornerTransforms =
        {
            new int[] { 0, 1, 2 },
            new int[] { 2, 0, 1 },
            new int[] { 1, 2, 0 },
            new int[] { 0, 2, 1 },
            new int[] { 2, 1, 0 },
            new int[] { 1, 0, 2 }
        };

        private readonly static int[][] s_EdgeTransforms =
        {
            new int[] { 0, 1, 2 },
            new int[] { 2, 0, 1 },
            new int[] { 1, 2, 0 },
            new int[] { 2, 1, 0 },
            new int[] { 1, 0, 2 },
            new int[] { 0, 2, 1 }
        };

        public record class Option(Vector2[] TexCoords, bool[] Connected) { }

        private readonly ITexturePage _texture;
        private readonly Option[] _options;

        public EdgeLibrary(ITexturePage texture, Option[] options)
        {
            _texture = texture;
            _options = options; 
        }

        public Texture GetTexture()
        {
            return _texture.GetTexture();
        }

        public IEnumerable<Option> Query(bool[] connected)
        {
            foreach (var option in _options)
            {
                for (int i = 0; i < s_CornerTransforms.Length; ++i)
                {
                    var transformed = 
                        new Option(
                            Transform(option.TexCoords, s_CornerTransforms[i]), 
                            Transform(option.Connected, s_EdgeTransforms[i]));
                    if (Satisfies(connected, transformed.Connected))
                    {
                        yield return transformed;
                    }
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

        private bool Satisfies<T>(T[] requirement, T[] option)
        {
            for (int i=0; i<option.Length; ++i)
            {
                if (!Equals(requirement[i], option[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

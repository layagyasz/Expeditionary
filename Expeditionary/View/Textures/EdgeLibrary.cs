using Cardamom.Graphics;
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

        public record class Option(Vector2[] TexCoords, bool[] Connected);

        public Texture Texture { get; }
        public Option[] Options { get; }

        public EdgeLibrary(Texture texture, Option[] options)
        {
            Texture = texture;
            Options = options; 
        }

        public IEnumerable<Option> Query(bool[] connected)
        {
            foreach (var option in Options)
            {
                for (int i = 0; i < s_CornerTransforms.Length; ++i)
                {
                    var transformed = 
                        new Option(
                            Utils.Transform(option.TexCoords, s_CornerTransforms[i]), 
                            Utils.Transform(option.Connected, s_EdgeTransforms[i]));
                    if (Satisfies(connected, transformed.Connected))
                    {
                        yield return transformed;
                    }
                }
            }
        }

        private static bool Satisfies<T>(T[] requirement, T[] option)
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

namespace Expeditionary.View.Textures
{
    public static class Utils
    {
        public static T[] Rotate<T>(T[] array, int rotation)
        {
            var result = new T[array.Length];
            for (int i=0; i<array.Length; i++)
            {
                result[i] = array[(i + rotation) % array.Length];
            }
            return result;
        }

        public static T[] Transform<T>(T[] array, int[] transform)
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

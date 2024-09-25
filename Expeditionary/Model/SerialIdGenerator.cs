namespace Expeditionary.Model
{
    public class SerialIdGenerator : IIdGenerator
    {
        private int _next;

        public int Next()
        {
            return _next++;
        }
    }
}

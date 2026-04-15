namespace Expeditionary.Model
{
    public class StaticIdGenerator : IIdGenerator
    {
        private readonly int _id;

        public StaticIdGenerator(int id)
        {
            _id = id;
        }

        public int Next()
        {
            return _id;
        }
    }
}

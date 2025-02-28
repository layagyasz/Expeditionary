namespace Expeditionary.Model.Missions
{
    public class SetupContext
    {
        public Random Random { get; }
        public IIdGenerator IdGenerator { get; }

        public SetupContext(Random random, IIdGenerator idGenerator)
        {
            Random = random;
            IdGenerator = idGenerator;
        }
    }
}

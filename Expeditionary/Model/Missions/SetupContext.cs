namespace Expeditionary.Model.Missions
{
    public class SetupContext
    {
        public Player Player { get; }
        public Random Random { get; }
        public IIdGenerator IdGenerator { get; }
        public bool IsTest { get; }

        public SetupContext(Player player, Random random, IIdGenerator idGenerator, bool isTest)
        {
            Player = player;
            Random = random;
            IdGenerator = idGenerator;
            IsTest = isTest;
        }
    }
}

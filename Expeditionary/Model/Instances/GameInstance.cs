using Expeditionary.Model.Formations;
using Expeditionary.Model.Galaxies;

namespace Expeditionary.Model.Instances
{
    public class GameInstance
    {
        public IIdGenerator IdGenerator { get; }
        public InstancePlayer Player { get; }
        public Galaxy Galaxy { get; }
        public MissionManager Missions { get; }
        public IEnumerable<InstanceMatchReport> MatchHistory => _matchHistory;

        private readonly List<InstanceMatchReport> _matchHistory = new();

        public GameInstance(IIdGenerator idGenerator, InstancePlayer player, Galaxy galaxy, MissionManager missions)
        {
            IdGenerator = idGenerator;
            Player = player;
            Galaxy = galaxy;
            Missions = missions;
        }

        public void AddFormation(TemplateFormation template)
        {
            Player.AddFormation(InstanceFormation.From(template, IdGenerator));
        }
    }
}

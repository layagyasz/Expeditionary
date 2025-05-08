using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public class RootFormationHandler : FormationHandlerBase
    {
        public override string Id => Player.Id.ToString();
        public Player Player { get; }

        public RootFormationHandler(Player player, IEnumerable<SimpleFormationHandler> children) 
            : base(children)
        { 
            Player = player;
        }

        public override IEnumerable<UnitHandler> GetUnitHandlers()
        {
            yield break;
        }
    }
}

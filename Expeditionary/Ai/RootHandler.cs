using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public class RootHandler : FormationHandlerBase
    {
        public override string Id => Player.Id.ToString();
        public override int Echelon => Children.Max(x => x.Echelon) + 1;
        public Player Player { get; }
        public  override IEnumerable<DiadHandler> Diads => Enumerable.Empty<DiadHandler>();

        public RootHandler(Player player, IEnumerable<FormationHandler> children) 
            : base(children)
        { 
            Player = player;
        }
    }
}

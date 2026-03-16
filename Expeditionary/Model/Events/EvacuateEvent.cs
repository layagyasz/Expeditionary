using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Events
{
    public record class EvacuateEvent(IEventSchedule Schedule, IMapRegion Region, Player Player, UnitTag Tag) : IEvent
    {
        public EventStatus Fire(Match match, TurnInfo turn)
        {
            if (!Schedule.IsDue(turn))
            {
                return EventStatus.Failed;
            }
            foreach (var asset in match.GetAssets()
                .Where(asset => asset.IsActive 
                    && !asset.IsPassenger 
                    && asset.Tags.Contains(Tag)
                    && Region.Contains(match.GetMap(), asset.Position)))
            {
                if (asset is Unit unit && unit.Player != Player)
                {
                    continue;
                }
                match.Evacuate(asset);
            }
            return EventStatus.Progressed;
        }
    }
}

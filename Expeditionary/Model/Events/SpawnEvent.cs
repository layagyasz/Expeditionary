using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Events
{
    public record class SpawnEvent(IEventSchedule Schedule, IMapRegion Region, Formation Formation, int Count) 
        : IEvent
    {
        private readonly IEnumerator<Unit> _enumerator = Formation.GetUnits().GetEnumerator();

        public EventStatus Fire(Match match, TurnInfo turn)
        {
            if (!Schedule.IsDue(turn))
            {
                return EventStatus.Failed;
            }
            var range = Region.Range(match.GetMap()).ToList();
            int i = 0;
            while (i < Count && _enumerator.MoveNext())
            {
                var unit = _enumerator.Current;
                match.Place(unit, range[match.GetRandom().Next(range.Count)]);
                i++;
            }
            return i == Count ? EventStatus.Progressed : EventStatus.Done;
        }
    }
}

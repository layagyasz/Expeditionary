using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Matches.Events
{
    public record class SpawnEvent(IEventSchedule Schedule, IMapRegion Region, Formation Formation, int Count)
        : IEvent
    {
        private readonly IEnumerator<Formation.Diad> _enumerator = Formation.GetDiads().GetEnumerator();

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
                var diad = _enumerator.Current;
                foreach (var unit in diad.GetUnits())
                {
                    match.Place(unit, range[match.GetRandom().Next(range.Count)]);
                }
                i++;
            }
            return i == Count ? EventStatus.Progressed : EventStatus.Done;
        }
    }
}

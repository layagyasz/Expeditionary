using Expeditionary.Model.Mapping.Appearance;

namespace Expeditionary.Model.Missions
{
    public record class MissionContent(MapSetup Map, List<PlayerSetup> Players)
    {
        public (Match, MapAppearance) Create(Random random, CreationContext context)
        {
            (var map, var appearance) = Map.GenerateMap(random);
            var match = new Match(new(), new SerialIdGenerator(), map);
            foreach (var player in Players)
            {
                player.Create(match, context);
            }
            return (match, appearance);
        }

        public void Setup(Match match, SetupContext context)
        {
            foreach (var player in Players)
            {
                player.Setup(match, context);
            }
        }
    }
}

using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Generator;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions
{
    public record class Mission(MapEnvironment MapEnvironment, Vector2i MapSize, List<PlayerSetup> Players)
    {
        public Match Setup(SetupContext context)
        {
            var map = MapGenerator.Generate(MapEnvironment.Parameters, MapSize, seed: context.Random.Next());
            var match = new Match(new SerialIdGenerator(), map);
            foreach (var player in Players)
            {
                player.Setup(match, context);
            }
            return match;
        }
    }
}

using Expeditionary.Evaluation;
using Expeditionary.Model.Mapping.Appearance;

namespace Expeditionary.Model.Missions
{
    public record class Mission(MapSetup Map, List<PlayerSetup> Players)
    {
        public (Match, MapAppearance) Setup(SetupContext context)
        {
            (var map, var appearance) = Map.GenerateMap(context);
            var match = new Match(new(), new SerialIdGenerator(), map);
            var evaluationCache = new EvaluationCache(map);
            foreach (var player in Players)
            {
                player.Setup(match, new PlayerSetupContext(context, evaluationCache));
            }
            return (match, appearance);
        }
    }
}

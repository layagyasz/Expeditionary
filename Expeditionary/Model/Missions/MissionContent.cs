using Expeditionary.Loader;
using Expeditionary.Model.Mapping.Appearance;

namespace Expeditionary.Model.Missions
{
    public record class MissionContent(MapSetup Map, List<PlayerSetup> Players)
    {
        private static readonly object o_Create = new();
        private static readonly object o_Setup = new();

        public LoaderTaskNode<(Match, MapAppearance)> Create(Random random, CreationContext context)
        {
            context.Status.AddSegment(o_Create);
            return Map.GenerateMap(context.Status, random)
                .Map(x =>
                    {
                        (var map, var appearance) = x;
                        var match = new Match(new(), new SerialIdGenerator(), x.Item1);
                        CreateAux(match, context);
                        return (match, appearance);
                    });
        }

        public LoaderTaskNode<Match> Setup(Match match, SetupContext context)
        {
            context.Status.AddSegments(o_Setup);
            return new SourceLoaderTask<Match>(() => { SetupAux(match, context); return match; }, isGL: false);
        }

        private void CreateAux(Match match, CreationContext context)
        {
            context.Status.AddWork(o_Create, Players.Count);
            foreach (var player in Players)
            {
                player.Create(match, context);
                context.Status.DoWork(o_Create);
            }
        }

        private void SetupAux(Match match, SetupContext context)
        {
            context.Status.AddWork(o_Setup, Players.Count);
            foreach (var player in Players)
            {
                player.Setup(match, context);
                context.Status.DoWork(o_Setup);
            }
        }
    }
}

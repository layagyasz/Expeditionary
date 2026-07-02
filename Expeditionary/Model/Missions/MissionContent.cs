using Cardamom.Utils.Suppliers.Promises;
using Expeditionary.Loader;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Matches;

namespace Expeditionary.Model.Missions
{
    public record class Mission(string NameKey, MapSetup Map, List<PlayerSetup> Players)
    {
        private static readonly object CreateSegment = new();
        private static readonly object SetupSegment = new();

        public LoaderTaskNode<(Match, MapAppearance)> Create(LoaderStatus status, CreationContext context)
        {
            status.AddSegment(CreateSegment);
            return Map.GenerateMap(status, context.Random)
                .Map(x =>
                    {
                        (var map, var appearance) = x;
                        var match = new Match(context.Random, new SerialIdGenerator(), x.Item1);
                        CreateAux(status, match, context);
                        return (match, appearance);
                    });
        }

        public LoaderTaskNode<Match> Setup(LoaderStatus status, IPromise<Match> match, IPromise<SetupContext> context)
        {
            status.AddSegments(SetupSegment);
            return new SourceLoaderTask<Match>(
                () => { SetupAux(status, match.Get(), context.Get()); return match.Get(); }, isGL: false);
        }

        private void CreateAux(LoaderStatus status, Match match, CreationContext context)
        {
            status.AddWork(CreateSegment, Players.Count);
            foreach (var player in Players)
            {
                player.Create(match, context);
                status.DoWork(CreateSegment);
            }
        }

        private void SetupAux(LoaderStatus status, Match match, SetupContext context)
        {
            status.AddWork(SetupSegment, Players.Count);
            foreach (var player in Players)
            {
                player.Setup(match, context);
                status.DoWork(SetupSegment);
            }
        }
    }
}

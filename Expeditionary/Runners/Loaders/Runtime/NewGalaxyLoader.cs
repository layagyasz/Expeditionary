using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Generator;
using System.Collections.Immutable;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewGalaxyLoader
    {
        private static readonly object s_Galaxy = new();
        private static readonly int s_Rounds = 10;

        public static (LoaderStatus, LoaderTaskNode<MissionManager>) Load(GameModule module, int seed)
        {
            var status = new LoaderStatus(new List<object>() { s_Galaxy }, logLength: 1);
            return new(status, new SourceLoaderTask<MissionManager>(() => Create(status, module, seed), isGL: false));
        }

        private static MissionManager Create(LoaderStatus status, GameModule module, int seed)
        {
            var random = new Random(seed);
            var missionGenerator =
                new MissionGenerator(
                    module.Galaxy,
                    module.MapEnvironmentGenerator,
                    new(module.FactionFormations, module.Formations));
            var missionNode =
                new MissionNode()
                {
                    Environment = new RandomEnvironmentProvider()
                    {
                        Sectors = ImmutableList.Create(1, 2, 3, 4, 5)
                    },
                    Difficulty = new() { MissionDifficulty.Medium },
                    Scale = new() { MissionScale.Medium },
                    Attackers = new() { module.Factions["faction-sm"] },
                    Defenders = new() { module.Factions["faction-earth"] },
                    Frequency = 1f,
                    Cap = 2,
                    Duration = new NormalSampler(10, 3),
                    Content =
                        new AssaultMissionGenerator()
                        {
                            ZoneOptions =
                            new()
                            {
                                new()
                                {
                                    CoreCount = 1,
                                    CandidateDensity = .005f,
                                    Type = StructureType.Mining,
                                    Level = 1,
                                    Size = new NormalSampler(2, 1),
                                    RiverPenalty = new(),
                                    CoastPenalty = new(),
                                    SlopePenalty = new(0, -1, 1),
                                    ElevationPenalty = new(0, -1, 1),
                                }
                            }
                        }
                };
            var missionManager = new MissionManager(missionGenerator, new List<MissionNode>() { missionNode }, random);
            status.AddWork(s_Galaxy, s_Rounds);
            for (int i = 0; i < s_Rounds; ++i)
            {
                missionManager.Step();
                status.DoWork(s_Galaxy);
            }
            return missionManager;
        }
    }
}

using Cardamom.Collections;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Generator;
using System.Collections.Immutable;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewGameInstanceLoader
    {
        private static readonly int i_Rounds = 10;
        private static readonly object o_Segment = new();

        public static (LoaderStatus, LoaderTaskNode<GameInstance>) Load(
            GameModule module, GameInstanceParameters parameters)
        {
            var status = new LoaderStatus(logLength: 1);
            status.AddSegment(o_Segment);
            return new(
                status, new SourceLoaderTask<GameInstance>(() => Create(status, module, parameters), isGL: false));
        }

        private static GameInstance Create(LoaderStatus status, GameModule module, GameInstanceParameters parameters)
        {
            var random = new Random(parameters.Seed);
            var missionGenerator = new MissionGenerator(module.Galaxy, module.MapEnvironmentGenerator);
            var missionNode =
                new MissionNode()
                {
                    Environment = new RandomEnvironmentProvider()
                    {
                        Sectors = ImmutableList.Create(1, 2, 3, 4, 5)
                    },
                    Difficulty = new() { MissionDifficulty.Medium },
                    Scale = new() { MissionScale.Medium },
                    Attackers = new() { parameters.Faction },
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
                                        Level = 2,
                                        Size = new NormalSampler(2, 1),
                                        RiverPenalty = new(),
                                        CoastPenalty = new(),
                                        SlopePenalty = new(0, -1, 1),
                                        ElevationPenalty = new(0, -1, 1),
                                    }
                                }
                        }
                };
            var missionManager = 
                new MissionManager(
                    new List<Faction>() { parameters.Faction },
                    missionGenerator, 
                    new List<MissionNode>() { missionNode },
                    random);
            status.AddWork(o_Segment, i_Rounds);
            for (int i = 0; i < i_Rounds; ++i)
            {
                missionManager.Step();
                status.DoWork(o_Segment);
            }
            var instance =
                new GameInstance(
                    new SerialIdGenerator(), 
                    new InstancePlayer(id: 0, parameters.Faction), module.Galaxy, missionManager);
            var formationGenerator = new FormationGenerator(module.FactionFormations, module.Formations);
            instance.AddFormation(
                formationGenerator.Generate(
                    new(
                        Points: 20000,
                        Echelon: 5, 
                        parameters.Faction, 
                        EnumSet<FormationRole>.All(),
                        ImmutableList.Create<UnitConstraint>(), 
                        random)));
            return instance;
        }
    }
}

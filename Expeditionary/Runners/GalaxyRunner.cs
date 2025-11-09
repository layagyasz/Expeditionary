using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Generator;
using Expeditionary.View.Scenes;
using Expeditionary.View.Screens;
using System.Collections.Immutable;

namespace Expeditionary.Runners
{
    public class GalaxyRunner : UiRunner
    {
        public GalaxyRunner(ProgramConfig config)
            : base(config) { }

        protected override IRenderable MakeRoot(
            ProgramData data, UiElementFactory uiElementFactory, SceneFactory sceneFactory)
        {
            var module = data.Module;
            var random = new Random();
            var missionGenerator =
                new MissionGenerator(
                    data.Module.Galaxy,
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
            for (int i = 0; i < 10; ++i)
            {
                missionManager.Step();
            }

            var screen = new GalaxyScreen(new NoOpController(), sceneFactory.Create(missionManager));
            return screen;
        }
    }
}

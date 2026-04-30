using Cardamom.Collections;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Instances.Campaigns;
using Expeditionary.Model.Missions.Generator;
using System.Collections.Immutable;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewGameInstanceLoader
    {
        private static readonly int Rounds = 10;
        private static readonly object Segment = new();

        public static (LoaderStatus, LoaderTaskNode<GameInstance>) Load(
            GameModule module, GameInstanceParameters parameters)
        {
            var status = new LoaderStatus(logLength: 1);
            status.AddSegment(Segment);
            return new(
                status, new SourceLoaderTask<GameInstance>(() => Create(status, module, parameters), isGL: false));
        }

        private static GameInstance Create(LoaderStatus status, GameModule module, GameInstanceParameters parameters)
        {
            var random = new Random(parameters.Seed);
            var missionGenerator = new MissionGenerator(module.Galaxy, module.MapEnvironmentGenerator);
            var missionManager = 
                new MissionManager(
                    new List<Faction>() { parameters.Faction },
                    missionGenerator, 
                    module.Campaigns.Values,
                    CampaignState.Empty(),
                    random);
            status.AddWork(Segment, Rounds);
            for (int i = 0; i < Rounds; ++i)
            {
                missionManager.StepMissions();
                status.DoWork(Segment);
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

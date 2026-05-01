using Cardamom.Collections;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Instances.Campaigns;
using System.Collections.Immutable;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewGameInstanceLoader
    {
        private static readonly object FormationSegment = new();
        private static readonly object MissionSegment = new();
        private static readonly int Rounds = 10;

        public static (LoaderStatus, LoaderTaskNode<GameInstance>) Load(
            GameModule module, GameInstanceParameters parameters)
        {
            var status = new LoaderStatus(logLength: 1);
            status.AddSegment(FormationSegment);
            status.AddSegment(MissionSegment);
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
            var instance =
                new GameInstance(
                    new SerialIdGenerator(), 
                    new InstancePlayer(id: 0, parameters.Faction), module.Galaxy, missionManager);

            status.AddWork(FormationSegment, 1);
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
            status.DoWork(FormationSegment);

            status.AddWork(MissionSegment, Rounds);
            for (int i = 0; i < Rounds; ++i)
            {
                instance.Missions.StepCampaigns(instance);
                instance.Missions.StepMissions();
                status.DoWork(MissionSegment);
            }

            return instance;
        }
    }
}

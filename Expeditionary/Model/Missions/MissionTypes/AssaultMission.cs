using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Model.Missions.Deployments;
using Expeditionary.Model.Missions.Objectives;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.MissionTypes
{
    public class AssaultMission : IMissionType
    {
        public Mission Create(MissionParameters parameters, MissionGenerationResources resources)
        {
            int playerId = 0;
            var players = new List<PlayerSetup>();
            foreach (var attacker in parameters.Attackers)
            {
                var player = new Player(playerId++, Team: 0, attacker);
                var setup = 
                    new PlayerSetup(
                        player,
                        new(
                            new List<IObjective>()
                            {
                                new OccupyObjective(MapTag.Control1, new UnitQuantity(1, 1))
                            }),
                        new()
                        {
                            new(
                                resources.FormationGenerator.Generate(attacker, resources.Random), 
                                new RandomDeployment())
                        });
                players.Add(setup);
            }
            foreach (var defender in parameters.Attackers)
            {
                var player = new Player(playerId++, Team: 1, defender);
                var setup =
                    new PlayerSetup(
                        player,
                        new(
                            new List<IObjective>()
                            {
                                new InvertObjective(Team: 0)
                            }),
                        new()
                        {
                            new(
                                resources.FormationGenerator.Generate(defender, resources.Random),
                                new RandomDeployment())
                        });
                players.Add(setup);
            }
            return
                new Mission(
                    new MapSetup(
                        parameters.Environment,
                        new Vector2i(100, 100),
                        Enumerable.Empty<CityGenerator.LayerParameters>()),
                    players);
        }
    }
}

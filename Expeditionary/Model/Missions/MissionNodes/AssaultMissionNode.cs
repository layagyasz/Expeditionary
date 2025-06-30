using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Missions.MissionNodes;
using Expeditionary.Model.Missions.Objectives;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.MissionTypes
{
    public record class AssaultMissionNode : MissionNodeBase
    {
        public List<CityGenerator.LayerParameters> ZoneOptions { get; set; } = new();

        public override Mission Create(MissionGenerationResources resources)
        {
            int playerId = 0;
            var players = new List<PlayerSetup>();
            foreach (var attacker in Attackers)
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
                                new DefaultOffensiveAssignment(
                                    MapDirection.South,  new() { new TagMapRegion(MapTag.Control1)}))
                        });
                players.Add(setup);
            }
            foreach (var defender in Defenders)
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
                                new DefaultDefensiveAssignment(
                                    MapDirection.North, new() { new TagMapRegion(MapTag.Control1)}))
                        });
                players.Add(setup);
            }
            var zoneChoice = ZoneOptions[resources.Random.Next(ZoneOptions.Count)];
            return
                new Mission(
                    new MapSetup(
                        Environment!.Get(resources.Random),
                        new Vector2i(100, 100),
                        new List<CityGenerator.LayerParameters>()
                        { 
                            Materialize(
                                zoneChoice, MapTag.Control1, Hexagons.Cubic.HexagonalOffset.Instance.Wrap(new(50, 50)))
                        }),
                    players);
        }

        private static CityGenerator.LayerParameters Materialize(
            CityGenerator.LayerParameters parameters, MapTag tag, Vector3i center)
        {
            return new()
            {
                CoreCount = parameters.CoreCount,
                CoreDensity = parameters.CoreDensity,
                CandidateDensity = parameters.CandidateDensity,
                Size = parameters.Size,
                Type = parameters.Type,
                Level = parameters.Level,
                Tags = parameters.Tags.Concat(Enumerable.Repeat(tag, 1)).ToList(),
                Center = center,
                DistancePenalty = new(0, 0.2f, 0),
                SprawlPenalty = parameters.SprawlPenalty,
                SlopePenalty = parameters.SlopePenalty,
                ElevationPenalty = parameters.ElevationPenalty,
                CoastPenalty = parameters.CoastPenalty,
                RiverPenalty = parameters.RiverPenalty,
                SandPenalty = parameters.SandPenalty,
                ClayPenalty = parameters.ClayPenalty,
                SiltPenalty = parameters.SiltPenalty,
                HeatPenalty = parameters.HeatPenalty,
                MoisturePenalty = parameters.MoisturePenalty,
            };
        }
    }
}

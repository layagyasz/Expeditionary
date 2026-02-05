using Expeditionary.Ai.Assignments;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Missions.Objectives;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.Generator
{
    public record class AssaultMissionGenerator : IMissionContentGenerator
    {
        public List<CityGenerator.LayerParameters> ZoneOptions { get; set; } = new();

        public MissionContent Generate(MissionNode node, MissionGenerationResources resources)
        {
            int playerId = 0;
            var players = new List<PlayerSetup>();
            foreach (var attacker in node.Attackers)
            {
                var player = new Player(playerId++, Team: 0, attacker);
                var setup = 
                    new PlayerSetup(
                        player,
                        new OccupyObjective(MapTag.Control1, new AssetValue(1, 1), IsDefender: false),
                        new()
                        {
                            new(
                                resources.FormationGenerator.Generate(attacker, resources.Random), 
                                new DefaultOffensiveAssignment(
                                    MapDirection.North,  new() { new TagMapRegion(MapTag.Control1)}))
                        });
                players.Add(setup);
            }
            foreach (var defender in node.Defenders)
            {
                var player = new Player(playerId++, Team: 1, defender);
                var setup =
                    new PlayerSetup(
                        player,
                        new PreventTeamObjective(Team: 0),
                        new()
                        {
                            new(
                                resources.FormationGenerator.Generate(defender, resources.Random),
                                new DefaultDefensiveAssignment(
                                    MapDirection.South, new() { new TagMapRegion(MapTag.Control1)}))
                        });
                players.Add(setup);
            }
            var zoneChoice = ZoneOptions[resources.Random.Next(ZoneOptions.Count)];
            var environment = node.Environment!.Get(resources);
            return
                new MissionContent(
                    new MapSetup(
                        environment,
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

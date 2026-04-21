using Cardamom.Collections;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai.Assignments;
using Expeditionary.Model.Missions.Objectives;

using OpenTK.Mathematics;
using System.Collections.Immutable;

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
                var player = new MatchPlayer(playerId++, Team: 0, attacker);
                var setup = 
                    new PlayerSetup(
                        player,
                        Objective: GetOffenseObjective(),
                        Events: new(),
                        Formation: 
                            new(
                                new FormationParameters(
                                    Echelon: 5,
                                    attacker, 
                                    EnumSet<FormationRole>.All(), 
                                    ImmutableList.Create<UnitConstraint>(),
                                    resources.Random), 
                                new DefaultOffensiveAssignment(
                                    MapDirection.North,  new() { new TagMapRegion(MapTag.Control1)})));
                players.Add(setup);
            }
            foreach (var defender in node.Defenders)
            {
                var player = new MatchPlayer(playerId++, Team: 1, defender);
                var setup =
                    new PlayerSetup(
                        player,
                        Objective: GetDefenseObjective(),
                        Events: new(),
                        Formation: 
                            new(
                                new FormationParameters(
                                    Echelon: 5,
                                    defender,
                                    EnumSet<FormationRole>.All(),
                                    ImmutableList.Create<UnitConstraint>(),
                                    resources.Random),
                                new DefaultDefensiveAssignment(
                                    MapDirection.South, new() { new TagMapRegion(MapTag.Control1)})));
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
                            zoneChoice with
                            {
                                Tags = zoneChoice.Tags.Concat(Enumerable.Repeat(MapTag.Control1, 1)).ToList(),
                                Center = Hexagons.Cubic.HexagonalOffset.Instance.Wrap(new(50, 50)),
                                DistancePenalty = new(0, 0.2f, 0),
                            }
                        }),
                    players);
        }

        protected virtual IObjective GetOffenseObjective()
        {
            return new OccupyObjective(MapTag.Control1, new AssetValue(1, 1), IsDefender: false);
        }

        protected virtual IObjective GetDefenseObjective()
        {
            return new OccupyObjective(MapTag.Control1, new AssetValue(1, 1), IsDefender: true);
        }
    }
}

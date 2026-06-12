using Cardamom.Collections;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Reporting;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.Runners.GameStates;
using System.Collections.Immutable;

namespace Expeditionary.Runners
{
    public static class TestGameStateContexts
    {
        public static IGameStateContext.MatchSummaryContext GenerateMatchSummaryContext(GameModule module)
        {
            var random = new Random();
            var faction = module.Factions.First().Value;
            var player = new MatchPlayer(0, 0, faction);
            var formationGenerator = new FormationGenerator(module.FactionFormations, module.Formations);
            var formation = 
                MatchFormation.From(
                    formationGenerator.Generate(
                        new(
                            Points: 20000, 
                            Echelon: 5, 
                            faction,
                            EnumSet<FormationRole>.All(),
                            ImmutableList.Create<UnitConstraint>(), 
                            random)), 
                    player, 
                    new SerialIdGenerator());
            foreach (var unit in formation.GetUnits())
            {
                var result = random.NextSingle();
                if (result < 0.2f)
                {
                    unit.Status = MatchAssetStatus.Destroyed;
                    unit.Damage(unit.Number);
                }
                else if (result < 0.4f)
                {
                    unit.Status = MatchAssetStatus.Active;
                    unit.Damage((int)(5 * (result - 0.2f) * unit.Number));
                }
                else
                {
                    unit.Status = MatchAssetStatus.Active;
                }
            }
            var reports = ImmutableDictionary.CreateBuilder<MatchPlayer, PlayerReport>();
            reports.Add(
                player, new PlayerReport(ObjectiveStatus.Stalemate, new(), FormationReport.Generate(formation)));
            return new IGameStateContext.MatchSummaryContext(
                Instance: null, StageKey: null, player, new MatchReport(reports.ToImmutable()));
        }
    }
}

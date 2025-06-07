using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class DefaultOffensiveAssignment(MapDirection Direction) : IFormationAssignment
    {
        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            return AssignDeployment(formation, match, tileEvaluator);
        }

        private FormationAssignment AssignDeployment(
            IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var region = new EdgeMapRegion(Direction, 0.5f);
            var origin =
                region.Range(match.GetMap())
                    .MaxBy(x => TileConsiderations.Evaluate(
                        TileConsiderations.Combine(
                            TileConsiderations.Weight(0.1f, TileConsiderations.Noise(new())),
                            TileConsiderations.Roading),
                        x,
                        map));
            var exemplar =
                formation.GetAllUnitHandlers()
                    .GroupBy(x => x.Unit.Type)
                    .ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
            int distance = match.GetMap().GetAxisSize(Direction) >> 2;
            var extent = Pathing.GetPathField(map, origin, exemplar.Movement, TileConsiderations.None, distance);
            var sdf = DenseSignedDistanceField.FromPathField(extent, distance >> 1);
            return PointAssignment.SelectFrom(
                formation,
                map,
                new ExplicitMapRegion(extent.Select(x => x.Destination)),
                MapDirectionUtils.Invert(Direction),
                tileEvaluator,
                TileConsiderations.Edge(sdf, 0));
        }

        private static bool IsDeployed(IFormationHandler formation)
        {
            return formation.GetAllUnitHandlers().Any(x => x.Unit.IsDestroyed || x.Unit.Position != null);
        }
    }
}

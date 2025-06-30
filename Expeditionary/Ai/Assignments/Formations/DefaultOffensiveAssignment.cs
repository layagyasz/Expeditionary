using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class DefaultOffensiveAssignment(MapDirection Direction, List<IMapRegion> TargetRegions) 
        : IFormationAssignment
    {
        public IMapRegion OperatingRegion => CompositeMapRegion.Union(TargetRegions);

        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            if (IsDeployed(formation))
            {
                return new DefaultDefensiveAssignment(Direction, TargetRegions)
                    .Assign(formation, match, tileEvaluator);
            }
            else
            {
                return AssignDeployment(formation, match, tileEvaluator);
            }
        }

        public float Evaluate(FormationAssignment assignment, Match match)
        {
            return new DefaultDefensiveAssignment(Direction, TargetRegions).Evaluate(assignment, match);
        }

        private FormationAssignment AssignDeployment(
            IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var region = new EdgeMapRegion(Direction, 0.33f);
            var origin =
                region.Range(match.GetMap())
                    .MaxBy(x => TileConsiderations.Evaluate(
                        TileConsiderations.Combine(
                            (0.1f, TileConsiderations.Noise(new())),
                            (1, TileConsiderations.Roading)),
                        x,
                        map));
            var exemplar =
                formation.GetAllUnitHandlers()
                    .GroupBy(x => x.Unit.Type)
                    .ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
            int distance = 16;
            var extent =
                Pathing.GetPathField(map, origin, exemplar.Movement, TileConsiderations.None, distance)
                    .Where(x => region.Contains(map, x.Destination)).ToList();
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

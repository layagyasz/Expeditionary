using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class DefaultOffensiveAssignment(MapDirection Facing, List<IMapRegion> TargetRegions)
        : IAssignment
    {
        public IMapRegion Region => CompositeMapRegion.Union(TargetRegions);

        public AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            if (IsDeployed(formation))
            {
                return new DefaultDefensiveAssignment(Facing, TargetRegions)
                    .Assign(formation, match, tileEvaluator);
            }
            else
            {
                return AssignDeployment(formation, match, tileEvaluator);
            }
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            throw new NotImplementedException();
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return new DefaultDefensiveAssignment(Facing, TargetRegions).EvaluateRealization(realization, match);
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }

        private AssignmentRealization AssignDeployment(
            IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var region = new EdgeMapRegion(Facing, 0.33f);
            var origin =
                region.Range(match.GetMap())
                    .MaxBy(x => TileConsiderations.Evaluate(
                        TileConsiderations.Combine(
                            (0.1f, TileConsiderations.Noise(new())),
                            (1, TileConsiderations.Roading)),
                        x,
                        map));
            var exemplar =
                formation.GetAllDiads()
                    .GroupBy(x => x.Unit.Unit.Type)
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
                MapDirectionUtils.Invert(Facing),
                tileEvaluator,
                TileConsiderations.Edge(sdf, 0));
        }

        private static bool IsDeployed(IAiHandler formation)
        {
            return formation.GetAllDiads().Any(x => x.Unit.Unit.IsDestroyed || x.Unit.Unit.Position != null);
        }
    }
}

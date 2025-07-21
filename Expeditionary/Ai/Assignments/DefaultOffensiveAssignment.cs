using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
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
            var region = new EdgeMapRegion(MapDirectionUtils.Invert(Facing), 0.33f);
            var origin =
                AssignmentHelper.GetBest(
                    map, 
                    region, 
                    TileConsiderations.Combine(
                        (1f, TileConsiderations.Essential(
                            tileEvaluator.IsReachable(
                                formation.GetMaxHindrance(), TargetRegions.First().Range(map).First()))),
                        (0.1f, TileConsiderations.Noise(new())),
                        (1, TileConsiderations.Roading)));
            return PointAssignment.SelectFrom(
                formation,
                map,
                CompositeMapRegion.Intersect(
                    region, new PointMapRegion(origin, 2 * PointAssignment.GetSpacing(formation.Echelon))),
                Facing,
                tileEvaluator,
                TileConsiderations.None,
                null);
        }

        private static bool IsDeployed(IAiHandler formation)
        {
            return formation.GetAllDiads().Any(x => x.Unit.Unit.IsDestroyed || x.Unit.Unit.Position != null);
        }
    }
}

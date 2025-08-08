using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public record class DefaultOffensiveAssignment(MapDirection Facing, List<IMapRegion> TargetRegions)
        : IAssignment
    {
        public Vector3i Origin => default;
        public IMapRegion Region => CompositeMapRegion.Union(TargetRegions);

        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            if (IsDeployed(formation))
            {
                return new DefaultDefensiveAssignment(Facing, TargetRegions)
                    .Assign(formation, match);
            }
            else
            {
                return AssignDeployment(formation, match);
            }
        }

        public IEnumerable<float> EvaluateActions(IEnumerable<IUnitAction> action, Unit unit, Match match)
        {
            throw new NotImplementedException();
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return new DefaultDefensiveAssignment(Facing, TargetRegions).EvaluateRealization(realization, match);
        }

        public bool NotifyAction(Unit unit, IUnitAction action, Match match)
        {
            throw new NotImplementedException();
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }

        private AssignmentRealization AssignDeployment(
            IAiHandler formation, Match match)
        {
            var tileEvaluator = match.GetEvaluator();
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
                origin);
        }

        private static bool IsDeployed(IAiHandler formation)
        {
            return formation.GetAllDiads().Any(x => x.Unit.Unit.IsDestroyed || x.Unit.Unit.Position != null);
        }
    }
}

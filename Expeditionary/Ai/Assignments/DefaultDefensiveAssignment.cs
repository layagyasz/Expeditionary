using Cardamom.Trackers;
using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class DefaultDefensiveAssignment(MapDirection Facing, List<IMapRegion> DefenseRegions)
        : IAssignment
    {
        public IMapRegion Region => CompositeMapRegion.Union(DefenseRegions);

        public AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            if (formation is RootHandler root)
            {
                return new(
                    new()
                    {
                        { root.Children.First(), new DefaultDefensiveAssignment(Facing, DefenseRegions) }
                    },
                    new());
            }
            var map = match.GetMap();
            var eligibleOccupiers =
                new LinkedList<Quantity<FormationHandler>>(
                    formation.Children
                        .Where(x => x.Formation.Role == FormationRole.Infantry)
                        .Select(x =>
                            Quantity<FormationHandler>.Create(x, x.Formation.GetAliveUnitQuantity().Points))
                        .OrderBy(x => x.Value));
            var regions =
                DefenseRegions.Select(
                    x => Quantity<IMapRegion>.Create(x, AssignmentHelper.GetRequiredCoverage(x.Range(map).Count())));
            var result = new Dictionary<FormationHandler, IAssignment>();
            foreach (var region in regions)
            {
                if (!eligibleOccupiers.Any())
                {
                    break;
                }
                var assignments = Assign(eligibleOccupiers, region);
                foreach (var f in assignments)
                {
                    result.Add(f, new AreaAssignment(region.Key, Facing));
                }
            }
            var currentAssignment = new AssignmentRealization(result, new());
            var parentAssignment =
                new AreaAssignment(new EdgeMapRegion(MapDirectionUtils.Invert(Facing), 0.5f), Facing);
            var defensiveAssignment =
                parentAssignment.PartitionByFormations(eligibleOccupiers.Select(x => x.Key), map);
            var offensiveAssignment =
                parentAssignment.PartitionByFormations(
                    formation.Children.Where(x => x.Formation.Role != FormationRole.Infantry), map);

            return new AssignmentRealization.Builder()
                .AddAll(currentAssignment)
                .AddAll(defensiveAssignment)
                .AddAll(offensiveAssignment)
                .Build();
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            throw new NotImplementedException();
        }

        public float EvaluateRealization(AssignmentRealization assignment, Match match)
        {
            return DefenseRegions.Sum(x => EvaluateRegion(assignment, match, x));
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }

        private static float EvaluateRegion(AssignmentRealization assignment, Match match, IMapRegion region)
        {
            return Math.Min(1f, assignment.ChildFormationAssignments
                .Where(x => MapRegions.Intersects(x.Value.Region, region, match.GetMap()))
                .Sum(x => x.Key.Formation.GetAliveUnitQuantity().Points)
                / AssignmentHelper.GetRequiredCoverage(region.Range(match.GetMap()).Count()));
        }

        private static List<FormationHandler> Assign(
            LinkedList<Quantity<FormationHandler>> formations, Quantity<IMapRegion> region)
        {
            var result = new List<FormationHandler>();
            float coverage = 0;
            while (coverage < region.Value && formations.Any())
            {
                Quantity<FormationHandler> formation;
                if (!result.Any())
                {
                    formation = formations.First();
                    formations.RemoveFirst();
                }
                else
                {
                    formation = formations.Last();
                    formations.RemoveLast();
                }
                coverage += formation.Value;
                result.Add(formation.Key);
            }
            return result;
        }
    }
}

using Cardamom.Trackers;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class DefaultDefensiveAssignment(MapDirection DefendingDirection, List<IMapRegion> DefenseRegions)
        : IFormationAssignment
    {
        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            var map = match.GetMap();
            var eligibleOccupiers =
                new LinkedList<Quantity<SimpleFormationHandler>>(
                    formation.Children
                        .Where(x => x.Formation.Role == FormationRole.Infantry)
                        .Select(x => Quantity<SimpleFormationHandler>.Create(x, GetCoverage(x)))
                        .OrderBy(x => x.Value));
            var regions =
                DefenseRegions.Select(x => Quantity<IMapRegion>.Create(x, GetRequiredCoverage(x.Range(map).Count())));
            var result = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
            foreach (var region in regions)
            {
                if (!eligibleOccupiers.Any())
                {
                    break;
                }
                var assignments = Assign(eligibleOccupiers, region);
                foreach (var f in assignments)
                {
                    result.Add(f, new AreaAssignment(region.Key, MapDirectionUtils.Invert(DefendingDirection)));
                }
            }
            var currentAssignment = new FormationAssignment(result, new());
            var parentAssignment = 
                new AreaAssignment(
                    new EdgeMapRegion(DefendingDirection, 0.5f), MapDirectionUtils.Invert(DefendingDirection));
            var defensiveAssignment =
                parentAssignment.PartitionByFormations(eligibleOccupiers.Select(x => x.Key), map);
            var offensiveAssignment = 
                parentAssignment.PartitionByFormations(
                    formation.Children.Where(x => x.Formation.Role != FormationRole.Infantry), map);

            return FormationAssignment.Combine(currentAssignment, defensiveAssignment, offensiveAssignment);
        }

        private static List<SimpleFormationHandler> Assign(
            LinkedList<Quantity<SimpleFormationHandler>> formations, Quantity<IMapRegion> region)
        {
            var result = new List<SimpleFormationHandler>();
            float coverage = 0;
            while (coverage < region.Value && formations.Any())
            {
                Quantity<SimpleFormationHandler> formation;
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

        private static float GetCoverage(SimpleFormationHandler formation)
        {
            return GetCoverage(formation.Formation.Echelon);
        }

        private static float GetCoverage(int echelon)
        {
            return MathF.Pow(3, echelon - 1);
        }

        private static float GetRequiredCoverage(int tileCount)
        {
            return 1.3333333f * MathF.Sqrt(tileCount);
        }
    }
}

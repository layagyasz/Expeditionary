using Cardamom.Trackers;
using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

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
            var edgeRegion = new EdgeMapRegion(DefendingDirection, 0.5f);
            var keyVector = GetKeyVector(DefendingDirection);
            var facing = MapDirectionUtils.Invert(DefendingDirection);
            foreach ((var f, var r) in eligibleOccupiers
                .Select(x => x.Key).Zip(edgeRegion.Partition(map, keyVector, eligibleOccupiers.Count)))
            {
                result.Add(f, new AreaAssignment(r, facing));
            }
            foreach (var f in formation.Children.Where(x => x.Formation.Role != FormationRole.Infantry))
            {
                result.Add(f, new AreaAssignment(new EdgeMapRegion(DefendingDirection, 0.25f), facing));
            }

            // Handle top-echelon units
            return new(result, new Dictionary<UnitHandler, IUnitAssignment>());
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

        private static Vector2 GetKeyVector(MapDirection direction)
        {
            if (direction.HasFlag(MapDirection.North) || direction.HasFlag(MapDirection.South))
            {
                return Vector2.UnitX;
            }
            return Vector2.UnitY;
        }
    }
}

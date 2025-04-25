using Cardamom.Trackers;
using Expeditionary.Evaluation;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class DefaultDefensiveDeployment(MapDirection DefendingDirection, List<IMapRegion> DefenseRegions)
        : IDeployment
    {
        public void Setup(Formation formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            var map = match.GetMap();
            var eligibleOccupiers =
                new LinkedList<Quantity<Formation>>(
                    formation.ComponentFormations
                        .Where(x => x.Role == FormationRole.Infantry)
                        .Select(x => Quantity<Formation>.Create(x, GetCoverage(x)))
                        .OrderBy(x => x.Value));
            var regions =
                DefenseRegions.Select(x => Quantity<IMapRegion>.Create(x, GetRequiredCoverage(x.Range(map).Count())));
            foreach (var region in regions)
            {
                if (!eligibleOccupiers.Any())
                {
                    break;
                }
                var assignments = Assign(eligibleOccupiers, region);
                foreach (var f in assignments)
                {
                    new AreaDeployment(region.Key, MapDirectionUtils.Invert(DefendingDirection))
                        .Setup(f, match, evaluationCache, random);
                }
            }
            var edgeRegion = new EdgeMapRegion(DefendingDirection, 0.5f);
            var keyVector = GetKeyVector(DefendingDirection);
            var facing = MapDirectionUtils.Invert(DefendingDirection);
            foreach ((var f, var r) in eligibleOccupiers
                .Select(x => x.Key).Zip(edgeRegion.Partition(map, keyVector, eligibleOccupiers.Count)))
            {
                new AreaDeployment(r, facing).Setup(f, match, evaluationCache, random);
            }
            foreach (var f in formation.ComponentFormations.Where(x => x.Role != FormationRole.Infantry))
            {
                new AreaDeployment(new EdgeMapRegion(DefendingDirection, 0.25f), facing)
                    .Setup(f, match, evaluationCache, random);
            }
        }

        private static List<Formation> Assign(LinkedList<Quantity<Formation>> formations, Quantity<IMapRegion> region)
        {
            var result = new List<Formation>();
            float coverage = 0;
            while (coverage < region.Value && formations.Any())
            {
                Quantity<Formation> formation;
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

        private static float GetCoverage(Formation formation)
        {
            return GetCoverage(formation.Echelon);
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

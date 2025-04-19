using Cardamom.Trackers;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class DefaultDefensiveDeployment(MapDirection DefendingDirection, List<IMapRegion> DefenseRegions) 
        : IDeployment
    {
        public void Setup(IEnumerable<Formation> formations, Match match, PlayerSetupContext context)
        {
            var map = match.GetMap();
            var eligibleOccupiers =
                new LinkedList<Quantity<Formation>>(
                    formations
                        .SelectMany(x => x.ComponentFormations)
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
                        .Setup(Enumerable.Repeat(f, 1), match, context);
                }
            }
            new AreaDeployment(new EdgeMapRegion(DefendingDirection), MapDirectionUtils.Invert(DefendingDirection))
                .Setup(eligibleOccupiers.Select(x => x.Key), match, context);
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
    }
}

using Cardamom.Trackers;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class DefaultDefensiveDeployment(MapDirection DefendingDirection, List<IMapRegion> DefenseRegions) 
        : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, SetupContext context)
        {
            var map = match.GetMap();
            var eligibleOccupiers =
                new LinkedList<Quantity<FormationTemplate>>(
                    formation.ComponentFormations
                        .Where(x => x.Role == FormationRole.Infantry)
                        .Select(x => Quantity<FormationTemplate>.Create(x, GetCoverage(x)))
                        .OrderBy(x => x.Value));
            var regions =
                DefenseRegions.Select(x => Quantity<IMapRegion>.Create(x, GetRequiredCoverage(x.Range(map).Count())));
            foreach (var region in regions)
            {
                if (!eligibleOccupiers.Any())
                {
                    break;
                }
                var formations = Assign(eligibleOccupiers, region);
                foreach (var f in formations)
                {
                    new RandomDeployment(region.Key).Setup(f, player, match, context);
                }
            }
            new RandomDeployment(new EdgeMapRegion(DefendingDirection)).Setup(formation, player, match, context);
        }

        private static List<FormationTemplate> Assign(
            LinkedList<Quantity<FormationTemplate>> formations, Quantity<IMapRegion> region)
        {
            var result = new List<FormationTemplate>();
            float coverage = 0;
            while (coverage < region.Value && formations.Any())
            {
                Quantity<FormationTemplate> formation;
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

        private static float GetCoverage(FormationTemplate formation)
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

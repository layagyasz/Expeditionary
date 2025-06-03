using Cardamom.Collections;
using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments.Formations
{
    public static class AssignmentHelper
    {
        public static Vector3i GetBest(Map map, IMapRegion region, TileConsideration consideration)
        {
            return region.Range(map).ArgMax(x => TileConsiderations.Evaluate(consideration, x, map));
        }

        public static IEnumerable<Vector3i> GetTop(
            Map map, IMapRegion region, TileConsideration consideration, int count)
        {
            var tilesAndEvaluations =
                region.Range(map)
                    .Select(hex =>
                        (
                            hex,
                            score: TileConsiderations.Evaluate(consideration, hex, map)
                        ))
                    .ToList();
            while (tilesAndEvaluations.Count < count)
            {
                tilesAndEvaluations.AddRange(tilesAndEvaluations);
            }
            tilesAndEvaluations.Sort((x, y) => -x.score.CompareTo(y.score));
            return tilesAndEvaluations.Take(count).Select(x => x.hex);
        }

        private static void Assign(
            Match match,
            IEnumerable<UnitHandler> units,
            IMapRegion region,
            TileConsideration consideration,
            Dictionary<UnitHandler, IUnitAssignment> result)
        {
            foreach ((var unit, var hex) in 
                units.Zip(
                    GetTop(match.GetMap(), region, consideration, units.Count())).Select(x => (x.First, x.Second)))
            {
                result.Add(unit, new PositionAssignment(hex));
            }
        }
    }
}

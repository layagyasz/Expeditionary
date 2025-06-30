using Cardamom.Collections;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments.Formations
{
    public static class AssignmentHelper
    {
        private static readonly float s_CoverageCoefficient = 200f;

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

        public static float GetRequiredCoverage(int tileCount)
        {
            return s_CoverageCoefficient * MathF.Sqrt(tileCount);
        }
    }
}

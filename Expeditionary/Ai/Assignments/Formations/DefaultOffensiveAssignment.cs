using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class DefaultOffensiveAssignment(MapDirection Direction) : IFormationAssignment
    {
        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            var map = match.GetMap();
            var region = new EdgeMapRegion(Direction, 0.5f);
            var origin =
                region.Range(match.GetMap())
                    .MaxBy(x => TileConsiderations.Evaluate(
                        TileConsiderations.Combine(
                            TileConsiderations.Weight(0.1f, TileConsiderations.Noise(random)),
                            TileConsiderations.Roading(map)),
                        x,
                        map));
            var exemplar =
                formation.GetAllUnitHandlers()
                    .GroupBy(x => x.Unit.Type)
                    .ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
            int distance = match.GetMap().GetAxisSize(Direction) / 3;
            var extent = Pathing.GetPathField(map, origin, exemplar.Movement, TileConsiderations.None, distance);
            var sdf = SignedDistanceField.FromPathField(extent, distance >> 1);
            return AssignmentHelper.AssignInRegion(
                formation,
                match,
                sdf,
                new ExplicitMapRegion(extent.Select(x => x.Destination)),
                MapDirectionUtils.Invert(Direction),
                TileConsiderations.Combine(
                    TileConsiderations.Essential(TileConsiderations.Land),
                    TileConsiderations.Direction(origin, MapDirectionUtils.Invert(Direction)),
                    TileConsiderations.Forestation,
                    TileConsiderations.Urbanization,
                    TileConsiderations.Roading(match.GetMap()),
                    TileConsiderations.Weight(0.1f, TileConsiderations.Noise(random))),
                evaluationCache);
        }
    }
}

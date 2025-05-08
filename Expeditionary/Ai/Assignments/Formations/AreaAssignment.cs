using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class AreaAssignment(IMapRegion Region, MapDirection Facing) : IFormationAssignment
    {
        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            return AssignmentHelper.AssignInRegion(
                formation,
                match,
                SignedDistanceField.FromRegion(match.GetMap(), Region, 0, Facing),
                Region,
                Facing,
                TileConsiderations.Combine(
                    TileConsiderations.Essential(TileConsiderations.Land),
                    TileConsiderations.Forestation,
                    TileConsiderations.Urbanization,
                    TileConsiderations.Roading(match.GetMap()),
                    TileConsiderations.Weight(0.1f, TileConsiderations.Noise(random))),
                evaluationCache);
        }
    }
}

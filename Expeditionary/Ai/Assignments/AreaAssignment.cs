using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class AreaAssignment(IMapRegion Region, MapDirection Facing) : IAssignment
    {
        public AssignmentRealization Assign(
            IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            if (formation.Echelon <= 3)
            {
                return AssignLowEchelon(formation, match, tileEvaluator);
            }
            else
            {
                return AssignHighEchelon(formation, match);
            }
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            throw new NotImplementedException();
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return Math.Min(1f, realization.ChildFormationAssignments
                .Where(x => MapRegions.Intersects(x.Value.Region, Region, match.GetMap()))
                .Sum(x => x.Key.Formation.GetAliveUnitQuantity().Points)
                / AssignmentHelper.GetRequiredCoverage(Region.Range(match.GetMap()).Count()));
        }

        public void Place(UnitHandler unit, Match match)
        {
            throw new NotImplementedException();
        }

        public AssignmentRealization PartitionByChildren(IFormationHandler formation, Map map)
        {
            return PartitionByFormations(formation.Children, map);
        }

        public AssignmentRealization PartitionByFormations(IEnumerable<SimpleFormationHandler> formations, Map map)
        {
            var keyVector = GetKeyVector(Facing);
            // Handle top-echelon units
            return
                new(
                    formations.Zip(Region.Partition(map, keyVector, formations.Count()))
                        .ToDictionary(x => x.First, x => (IAssignment)new AreaAssignment(x.Second, Facing)),
                new());
        }

        private AssignmentRealization AssignHighEchelon(IFormationHandler formation, Match match)
        {
            return PartitionByChildren(formation, match.GetMap());
        }

        private AssignmentRealization AssignLowEchelon(
            IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            return PointAssignment.SelectFrom(
                formation,
                match.GetMap(),
                Region,
                Facing,
                tileEvaluator,
                TileConsiderations.Edge(DenseSignedDistanceField.FromRegion(match.GetMap(), Region, 0, Facing), 0));
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

using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public record class AreaAssignment(IMapRegion Region, Vector3i Origin, MapDirection Facing) : IAssignment
    {
        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            if (formation.Echelon <= 3)
            {
                return AssignLowEchelon(formation, match);
            }
            else
            {
                return AssignHighEchelon(formation, match);
            }
        }

        public IEnumerable<(IUnitAction, float)> EvaluateActions(
            IEnumerable<IUnitAction> action, Unit unit, Match match)
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

        public Vector3i SelectHex(Map map)
        {
            return Region.Range(map).First();
        }

        public AssignmentRealization PartitionByChildren(IAiHandler formation, Map map)
        {
            return PartitionByFormations(formation.Children, map);
        }

        public AssignmentRealization PartitionByFormations(IEnumerable<FormationHandler> formations, Map map)
        {
            var keyVector = GetKeyVector(Facing);
            // Handle top-echelon units
            return
                new(
                    formations.Zip(Region.Partition(map, keyVector, formations.Count()))
                        .ToDictionary(x => x.First, x => (IAssignment)new AreaAssignment(x.Second, Origin, Facing)),
                new());
        }

        private AssignmentRealization AssignHighEchelon(IAiHandler formation, Match match)
        {
            return PartitionByChildren(formation, match.GetMap());
        }

        private AssignmentRealization AssignLowEchelon(IAiHandler formation, Match match)
        {
            return PointAssignment.SelectFrom(
                formation,
                match.GetMap(),
                Region,
                Facing,
                match.GetEvaluator(),
                TileConsiderations.Edge(DenseSignedDistanceField.FromRegion(match.GetMap(), Region, 0, Facing), 0),
                Origin);
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

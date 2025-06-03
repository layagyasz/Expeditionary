using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class AreaAssignment(IMapRegion Region, MapDirection Facing) : IFormationAssignment
    {
        public FormationAssignment Assign(
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

        public FormationAssignment PartitionByChildren(IFormationHandler formation, Map map)
        {
            return PartitionByFormations(formation.Children, map);
        }

        public FormationAssignment PartitionByFormations(IEnumerable<SimpleFormationHandler> formations, Map map)
        {
            var keyVector = GetKeyVector(Facing);
            // Handle top-echelon units
            return
                new(
                    formations.Zip(Region.Partition(map, keyVector, formations.Count()))
                        .ToDictionary(x => x.First, x => (IFormationAssignment)new AreaAssignment(x.Second, Facing)),
                new());
        }

        private FormationAssignment AssignHighEchelon(IFormationHandler formation, Match match)
        {
            return PartitionByChildren(formation, match.GetMap());
        }

        private FormationAssignment AssignLowEchelon(
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

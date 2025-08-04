using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public record class RandomAssignment(Vector3i Origin, IMapRegion Region, MapDirection Facing) : IAssignment
    {
        public AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var options = Region.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            var formationResult = new Dictionary<FormationHandler, IAssignment>();
            var diadResult = new Dictionary<DiadHandler, IAssignment>();
            var random = new Random();
            foreach (var child in formation.Children)
            {
                formationResult.Add(child, this);
            }
            foreach (var diad in formation.Diads)
            {
                diadResult.Add(diad, new PointAssignment(options[random.Next(options.Count)], Origin, Region, Facing));
            }
            return new(formationResult, diadResult);
        }

        public IEnumerable<(IUnitAction, float)> EvaluateActions(
            IEnumerable<IUnitAction> actions, Unit unit, TileEvaluator tileEvaluator, Match match)
        {
            return UnitActionEvaluations.EvaluateDefault(actions, unit, tileEvaluator.GetEvaluatorFor(unit, Facing), match);
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 1f;
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }
    }
}

using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class RandomAssignment(IMapRegion Region, MapDirection Facing) : IAssignment
    {
        public AssignmentRealization Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var options = Region.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            var formationResult = new Dictionary<SimpleFormationHandler, IAssignment>();
            var unitResult = new Dictionary<UnitHandler, IAssignment>();
            var random = new Random();
            foreach (var child in formation.Children)
            {
                formationResult.Add(child, this);
            }
            foreach (var unit in formation.GetUnitHandlers())
            {
                unitResult.Add(unit, new PointAssignment(options[random.Next(options.Count)], Region, Facing));
            }
            return new(formationResult, unitResult);
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            return 0;
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 1f;
        }

        public void Place(UnitHandler unit, Match match)
        {
            throw new NotImplementedException();
        }
    }
}

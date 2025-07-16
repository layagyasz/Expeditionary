using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class RandomAssignment(IMapRegion Region, MapDirection Facing) : IAssignment
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
                diadResult.Add(diad, new PointAssignment(options[random.Next(options.Count)], Region, Facing));
            }
            return new(formationResult, diadResult);
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            throw new NotImplementedException();
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

using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class RandomAssignment(IMapRegion Region) : IFormationAssignment
    {
        public IMapRegion OperatingRegion => Region;

        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            var map = match.GetMap();
            var options = Region.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            var formationResult = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
            var unitResult = new Dictionary<UnitHandler, IUnitAssignment>();
            var random = new Random();
            foreach (var child in formation.Children)
            {
                formationResult.Add(child, this);
            }
            foreach (var unit in formation.GetUnitHandlers())
            {
                unitResult.Add(unit, new PositionAssignment(MapDirection.All, options[random.Next(options.Count)]));
            }
            return new(formationResult, unitResult);
        }

        public float Evaluate(FormationAssignment assignment, Match match)
        {
            return 1f;
        }
    }
}

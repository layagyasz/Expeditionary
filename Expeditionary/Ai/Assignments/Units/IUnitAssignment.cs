using Expeditionary.Ai.Actions;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments.Units
{
    public interface IUnitAssignment
    {
        MapDirection Facing { get; }
        void Place(UnitHandler unit, Match match);
        float Evaluate(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match);
    }
}

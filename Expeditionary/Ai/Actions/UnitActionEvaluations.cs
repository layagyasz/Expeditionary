using Expeditionary.Evaluation;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Model;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public static class UnitActionEvaluations
    {
        public static IEnumerable<(IUnitAction, float)> EvaluateDefault(
            IEnumerable<IUnitAction> actions, Unit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            return actions.Select(x => (x, EvaluateDefault(x, unit, match, tileEvaluator)));
        }

        public static float EvaluateDefault(
            IUnitAction action, Unit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            if (action is AttackAction attackAction)
            {
                return ActionEvaluation.EvaluateAttack(
                    unit, attackAction.Attack, attackAction.Mode, attackAction.Target, match.GetMap());
            }
            if (action is LoadAction)
            {
                return 0;
            }
            if (action is MoveAction moveAction)
            {
                return ActionEvaluation.EvaluateFreeMove(unit, moveAction.Path, match, tileEvaluator);
            }
            if (action is UnloadAction unloadAction)
            {
                return ActionEvaluation.EvaluateUnload(unit, match, tileEvaluator);
            }
            return 0;
        }
    }
}

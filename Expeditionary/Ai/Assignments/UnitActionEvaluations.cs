using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Units;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public static class UnitActionEvaluations
    {
        public static IEnumerable<(IUnitAction, float)> EvaluateDefault(
            IEnumerable<IUnitAction> actions, Unit unit, UnitTileEvaluator tileEvaluator, Match match)
        {
            return actions.Select(x => (x, EvaluateDefault(x, unit, tileEvaluator, match)));
        }

        public static float EvaluateDefault(
            IUnitAction action, Unit unit, UnitTileEvaluator tileEvaluator, Match match)
        {
            if (action is AttackAction attackAction)
            {
                return ActionEvaluation.EvaluateAttack(
                    unit, attackAction.Attack, attackAction.Mode, attackAction.Target, match.GetMap());
            }
            if (action is MoveAction moveAction)
            {
                return ActionEvaluation.EvaluateFreeMove(unit, moveAction.Path, tileEvaluator, match);
            }
            return 0;
        }
    }
}

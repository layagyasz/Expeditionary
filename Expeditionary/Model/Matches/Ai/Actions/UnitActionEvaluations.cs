using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Evaluation;
using Expeditionary.Model.Matches.Evaluation.TileEvaluators;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public static class UnitActionEvaluations
    {
        public static IEnumerable<float> EvaluateDefault(
            IEnumerable<IUnitAction> actions, Unit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            return actions.Select(x => EvaluateDefault(x, unit, match, tileEvaluator));
        }

        public static float EvaluateDefault(
            IUnitAction action, Unit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            if (action is DirectAttackAction directAttack)
            {
                return ActionEvaluation.EvaluateDirectAttack(
                    unit, directAttack.Attack, directAttack.Mode, directAttack.Target, match.GetMap());
            }
            if (action is IndirectAttackAction indirectAttack)
            {
                return ActionEvaluation.EvaluateIndirectAttack(
                    unit, indirectAttack.Attack, indirectAttack.Mode, indirectAttack.Target, match);
            }
            if (action is LoadAction)
            {
                return 0;
            }
            if (action is MoveAction moveAction)
            {
                return ActionEvaluation.EvaluateMove(unit, moveAction.Path, match, tileEvaluator);
            }
            if (action is UnloadAction)
            {
                return ActionEvaluation.EvaluateUnload(unit, match, tileEvaluator);
            }
            return 0;
        }
    }
}

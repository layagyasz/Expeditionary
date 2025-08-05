using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class MoveAction(Pathing.PathOption Path) : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            var realPath = 
                Pathing.GetShortestPath(
                    match.GetMap(), 
                    unit.Position!.Value, 
                    Path.Destination,
                    unit.Type.Movement,
                    TileConsiderations.None);
            return match.DoOrder(new MoveOrder(unit, realPath));
        }

        public static IEnumerable<IUnitAction> GenerateValidMoves(Match match, Unit unit)
        {
            return Pathing.GetPathField(
                match.GetMap(), unit.Position!.Value, unit.Type.Movement, TileConsiderations.None, unit.Type.Speed)
                .Where(x => x.Destination != unit.Position)
                .Select(x => new MoveAction(x));
        }
    }
}

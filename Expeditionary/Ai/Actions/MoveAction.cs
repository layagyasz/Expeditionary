using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class MoveAction(Pathing.PathOption Path) : IUnitAction
    {
        public void Do(Match match, Unit unit)
        {
            var realPath = 
                Pathing.GetShortestPath(
                    match.GetMap(), 
                    unit.Position!.Value, 
                    Path.Destination,
                    unit.Type.Movement,
                    TileConsiderations.None);
            match.DoOrder(new MoveOrder(unit, realPath));
        }

        public static IEnumerable<IUnitAction> GenerateValidMoves(Match match, Unit unit)
        {
            return Pathing.GetPathField(
                match.GetMap(), unit.Position!.Value, unit.Type.Movement, TileConsiderations.None, unit.Movement)
                .Where(x => x.Destination != unit.Position)
                .Select(x => new MoveAction(x));
        }
    }
}

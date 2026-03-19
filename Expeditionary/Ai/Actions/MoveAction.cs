using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Orders;

namespace Expeditionary.Ai.Actions
{
    public record class MoveAction(Pathing.PathOption Path) : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            var realPath = 
                Pathing.GetShortestPath(
                    match.GetMap(), 
                    unit.Position, 
                    Path.Destination,
                    unit.Type.Movement,
                    TileConsiderations.None);
            return match.DoOrder(new MoveOrder(unit, realPath));
        }

        public static IEnumerable<IUnitAction> GenerateValidMoves(Match match, Unit unit)
        {
            return Pathing.GetPathField(
                match.GetMap(), unit.Position, unit.Type.Movement, TileConsiderations.None, unit.Type.Speed)
                .Where(x => x.Destination != unit.Position)
                .Select(x => new MoveAction(x));
        }
    }
}

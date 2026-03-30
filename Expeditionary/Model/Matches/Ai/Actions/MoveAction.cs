using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Evaluation.Considerations;
using Expeditionary.Model.Matches.Orders;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public record class MoveAction(Pathing.PathOption Path) : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
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

        public static IEnumerable<IUnitAction> GenerateValidMoves(Match match, MatchUnit unit)
        {
            return Pathing.GetPathField(
                match.GetMap(), unit.Position, unit.Type.Movement, TileConsiderations.None, unit.Type.Speed)
                .Where(x => x.Destination != unit.Position)
                .Select(x => new MoveAction(x));
        }
    }
}

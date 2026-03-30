using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Orders
{
    public interface IOrder
    {
        MatchUnit Unit { get; }
        bool Validate(Match match);
        void Execute(Match match);
    }
}

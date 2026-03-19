using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Orders
{
    public interface IOrder
    {
        Unit Unit { get; }
        bool Validate(Match match);
        void Execute(Match match);
    }
}

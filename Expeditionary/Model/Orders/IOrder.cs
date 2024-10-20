using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Orders
{
    public interface IOrder
    {
        Unit Unit { get; }
        bool Validate(Match match);
        void Execute(Match match);
    }
}

namespace Expeditionary.Model.Missions
{
    public record struct UnitQuantity(int Number, int Points)
    {
        public static UnitQuantity operator +(UnitQuantity left, UnitQuantity right)
        {
            return new(left.Number + right.Number, left.Points + right.Points);
        }
        
        public static UnitQuantity operator *(float coefficient, UnitQuantity value)
        {
            return new((int)(coefficient * value.Number), (int)(coefficient * value.Points));
        }

        public static bool operator >(UnitQuantity left, UnitQuantity right)
        {
            return left.Number > right.Number && left.Points > right.Points;
        }

        public static bool operator >=(UnitQuantity left, UnitQuantity right)
        {
            return left > right || left == right;
        }

        public static bool operator <(UnitQuantity left, UnitQuantity right)
        {
            return left.Number < right.Number && left.Points < right.Points;
        }

        public static bool operator <=(UnitQuantity left, UnitQuantity right)
        {
            return left <= right || left == right;
        }
    }
}

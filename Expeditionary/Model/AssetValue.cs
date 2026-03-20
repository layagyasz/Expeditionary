namespace Expeditionary.Model
{
    public record struct AssetValue(int Number, int Points)
    {
        public static readonly AssetValue None = new(0, 0);

        public static AssetValue operator +(AssetValue left, AssetValue right)
        {
            return new(left.Number + right.Number, left.Points + right.Points);
        }

        public static AssetValue operator *(float coefficient, AssetValue value)
        {
            return new((int)(coefficient * value.Number), (int)(coefficient * value.Points));
        }

        public static bool operator >(AssetValue left, AssetValue right)
        {
            return left.Number > right.Number && left.Points > right.Points;
        }

        public static bool operator >=(AssetValue left, AssetValue right)
        {
            return left > right || left == right;
        }

        public static bool operator <(AssetValue left, AssetValue right)
        {
            return left.Number < right.Number && left.Points < right.Points;
        }

        public static bool operator <=(AssetValue left, AssetValue right)
        {
            return left <= right || left == right;
        }
    }
}

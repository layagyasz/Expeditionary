namespace Expeditionary.Model.Units
{
    public readonly record struct UnitBoundedValue
    {
        public static readonly UnitBoundedValue None = new();

        public Modifier Minimum { get; }
        public Modifier Value { get; }

        public UnitBoundedValue(Modifier minimum, Modifier value)
        {
            Minimum = minimum;
            Value = value;
        }

        public static UnitBoundedValue Add(UnitBoundedValue left, UnitBoundedValue right)
        {
            return new(left.Minimum + right.Minimum, left.Value + right.Value);
        }
    }
}

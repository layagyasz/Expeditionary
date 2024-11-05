namespace Expeditionary.Model.Combat.Units
{
    public struct UnitBoundedValue
    {
        public static readonly UnitBoundedValue None = new();

        public Modifier Minimum { get; set; }
        public Modifier Value { get; set; }


        public UnitBoundedValue() { }

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

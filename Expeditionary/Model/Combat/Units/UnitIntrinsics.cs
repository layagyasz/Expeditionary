namespace Expeditionary.Model.Combat.Units
{
    public class UnitIntrinsics
    {
        public UnitModifier Mass { get; set; }
        public UnitModifier Power { get; set; }
        public UnitModifier Profile { get; set; }

        public float GetSpeed()
        {
            return Mass.GetValue() / Power.GetValue();
        }
    }
}

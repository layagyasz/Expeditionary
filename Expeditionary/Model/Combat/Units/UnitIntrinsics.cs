namespace Expeditionary.Model.Combat.Units
{
    public class UnitIntrinsics
    {
        public UnitModifier Mass { get; set; }
        public UnitModifier Morale { get; set; }
        public UnitModifier Number { get; set; }
        public UnitModifier Power { get; set; }
        public UnitModifier Profile { get; set; }
        public UnitModifier Stamina { get; set; }

        public float GetSpeed()
        {
            return Mass.GetValue() / Power.GetValue();
        }
    }
}

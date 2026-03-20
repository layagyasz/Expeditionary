namespace Expeditionary.Model.Units
{
    public record struct UnitWeaponRange(Modifier Physical, Modifier Targeting, Modifier Minimum)
    {
        public float GetMaximum()
        {
            return Math.Min(Physical.GetValue(), Targeting.GetValue());
        }

        public float GetMinimum()
        {
            return Minimum.GetValue();
        }
    }
}

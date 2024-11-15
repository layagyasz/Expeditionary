namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitMassComponent
    {
        public Modifier Density { get; set; }
        public Modifier Amount { get; set; }

        public float GetValue()
        {
            return Density.GetValue() * Amount.GetValue();
        }
    }
}

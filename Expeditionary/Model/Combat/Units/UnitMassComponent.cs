namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitMassComponent(Modifier Density, Modifier Amount)
    {
        public float GetValue()
        {
            return Density.GetValue() * Amount.GetValue();
        }
    }
}

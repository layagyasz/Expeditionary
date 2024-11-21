namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitMass(UnitMassComponent Armor, UnitMassComponent Body, UnitMassComponent Equipment)
    {
        public float GetValue()
        {
            return Armor.GetValue() + Body.GetValue() + Equipment.GetValue();
        }
    }
}

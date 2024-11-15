namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitMass
    {
        public UnitMassComponent Armor { get; set; }
        public UnitMassComponent Body { get; set; }
        public UnitMassComponent Equipment { get; set; }

        public float GetValue()
        {
            return Armor.GetValue() + Body.GetValue() + Equipment.GetValue();
        }
    }
}

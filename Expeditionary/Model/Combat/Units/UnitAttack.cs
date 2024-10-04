namespace Expeditionary.Model.Combat.Units
{
    public class UnitAttack
    {
        public UnitModifier Volume { get; set; }
        public UnitModifier Range { get; set; }
        public UnitModifier Accuracy { get; set; }
        public UnitModifier Tracking { get; set; }
        public UnitModifier Penetration { get; set; }
        public UnitModifier Lethality { get; set; }
    }
}

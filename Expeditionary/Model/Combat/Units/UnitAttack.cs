namespace Expeditionary.Model.Combat.Units
{
    public record class UnitAttack
    {
        public bool IsDistributed { get; set; }
        public int Number { get; set; }
        public CombatCondition Condition { get; set; }
        public Modifier Volume { get; set; }
        public Modifier Range { get; set; }
        public Modifier Accuracy { get; set; }
        public Modifier Tracking { get; set; }
        public Modifier Penetration { get; set; }
        public Modifier Lethality { get; set; }
    }
}

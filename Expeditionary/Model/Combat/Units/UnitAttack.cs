namespace Expeditionary.Model.Combat.Units
{
    public class UnitAttack
    {
        public Modifier Volume { get; set; }
        public Modifier Range { get; set; }
        public Modifier Accuracy { get; set; }
        public Modifier Tracking { get; set; }
        public Modifier Penetration { get; set; }
        public Modifier Lethality { get; set; }
    }
}

namespace Expeditionary.Model.Combat
{
    public class UnitDefenseEnvelope
    {
        public UnitDefense Profile { get; set; }
        public UnitDefense Maneuver { get; set; }
        public UnitDefense Armor { get; set; }
        public UnitDefense Vitality { get; set; }
    }
}

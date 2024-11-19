using Expeditionary.Model.Combat;

namespace Expeditionary.Model.Mapping
{
    public record class Tile
    {
        public int Elevation { get; set; }
        public int Slope { get; set; }
        public float Heat { get; set; }
        public float Moisture { get; set; }
        public Terrain Terrain { get; set; } = new();
        public Movement.Hindrance Hindrance { get; set; }
        public Structure Structure { get; set; }

        public CombatCondition GetConditions()
        {
            return IsUrban() ? CombatCondition.Urban : CombatCondition.None;
        }

        public bool IsUrban()
        {
            return Structure.Type != StructureType.None && Structure.Type != StructureType.Agricultural;
        }
    }
}

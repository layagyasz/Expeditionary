using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat.Units
{
    public class Unit
    {
        public int Id { get; }
        public Vector3i Position { get; set; }
        public UnitType Type { get; }

        public Unit(int id, UnitType type)
        {
            Id = id;
            Type = type;
        }
    }
}

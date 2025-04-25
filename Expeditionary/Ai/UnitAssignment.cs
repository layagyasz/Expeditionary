using Expeditionary.Model.Formations;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai
{
    public class UnitAssignment
    {
        public Unit Unit { get; }
        public FormationRole Role { get; }
        public Vector3i Position { get; set; }

        public UnitAssignment(Unit unit, FormationRole role)
        {
            Unit = unit;
            Role = role;
        }

        public bool IsActive()
        {
            return !Unit.IsDestroyed && Unit.Position != null;
        }
    }
}

using Expeditionary.Model;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments.Units
{
    public record class PositionAssignment(Vector3i Position) : IUnitAssignment
    {
        public void Place(UnitAssignment unit, Match match)
        {
            match.Place(unit.Unit, Position);
        }
    }
}

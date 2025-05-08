using Cardamom.Logging;
using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class UnitHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(UnitHandler));

        public Unit Unit { get; }
        public FormationRole Role { get; }
        public IUnitAssignment Assignment { get; private set; } = new NoUnitAssignment();

        public UnitHandler(Unit unit, FormationRole role)
        {
            Unit = unit;
            Role = role;
        }

        public void SetAssignment(IUnitAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Unit.Id).Log($"assigned {assignment}");
        }

        public bool IsActive()
        {
            return !Unit.IsDestroyed && Unit.Position != null;
        }
    }
}

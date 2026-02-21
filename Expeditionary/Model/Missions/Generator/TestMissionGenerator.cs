using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions.Generator
{
    public record class TestMissionGenerator : AssaultMissionGenerator
    {
        protected override IObjective GetOffenseObjective()
        {
            return new DurationObjective(1);
        }

        protected override IObjective GetDefenseObjective()
        {
            return new DurationObjective(1);
        }

    }
}

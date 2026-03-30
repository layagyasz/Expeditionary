using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Matches.Reporting
{
    public record class UnitReport(int InstanceId, UnitType Type, MatchAssetStatus Status)
    {
        public static UnitReport Generate(MatchUnit unit)
        {
            return new(unit.InstanceId, unit.Type, unit.Status);
        }
    }
}

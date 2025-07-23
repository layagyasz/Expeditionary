using Expeditionary.Model.Units;

namespace Expeditionary.Model
{
    public static class OrderChecker
    {
        public static bool CanLoad(Unit unit, IAsset asset)
        {
            if (unit.Actions <= 0)
            {
                return false;
            }
            if (unit.Position != asset.Position)
            {
                return false;
            }
            if (asset is Unit target)
            {
                if (!CanLoad(unit.Type, target.Type))
                {
                    return false;
                }
                return unit.Player == target.Player
                    && unit.Passenger == null
                    && !unit.IsPassenger
                    && !target.IsPassenger
                    && target.Actions > 0;
            }
            // TODO: Handle other kinds of assets.
            return false;
        }

        public static bool CanLoad(UnitType type, UnitType target)
        {
            bool isCarried = target.Tags.Contains(UnitTag.Infantry);
            bool isTowed = target.Tags.Contains(UnitTag.Towed);
            if (!isCarried && !isTowed)
            {
                return false;
            }
            if (isCarried && !type.Tags.Contains(UnitTag.Transport))
            {
                return false;
            }
            if (isTowed && !type.Tags.Contains(UnitTag.Tractor))
            {
                return false;
            }
            return true;
        }
    }
}

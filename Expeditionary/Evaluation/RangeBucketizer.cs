using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public static class RangeBucketizer
    {
        public static RangeBucket ToBucket(UnitType unitType)
        {
            if (!unitType.Weapons.Any())
            {
                return RangeBucket.Short;
            }
            return ToBucket(unitType.Weapons.Select(x => x.Weapon).SelectMany(x => x.Modes).Max(x => x.Range.Get()));
        }

        public static RangeBucket ToBucket(float range)
        {
            if (range >= 8)
            {
                return RangeBucket.Long;
            }
            if (range >= 4)
            {
                return RangeBucket.Medium;
            }
            return RangeBucket.Short;
        }

        public static int ToRange(RangeBucket bucket)
        {
            return 1 << (int)bucket;
        }
    }
}

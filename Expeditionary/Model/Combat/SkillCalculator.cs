namespace Expeditionary.Model.Combat
{
    public static class SkillCalculator
    {
        public static float RangeAttenuate(float value, float range, float distance)
        {
            return Math.Max(0, value * (1f - distance / (range + 1)));
        }

        public static float SignatureAttenuate(float value, float signature)
        {
            return MathF.Sqrt(value * signature);
        }
    }
}

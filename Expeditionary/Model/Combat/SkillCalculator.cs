namespace Expeditionary.Model.Combat
{
    public static class SkillCalculator
    {
        private static readonly float s_VolumeConstant = 8f;

        public static float ArmamentAttenuate(float value)
        {
            if (value > 4)
            {
                return 0.5f * value;
            }
            return value;
        }

        public static float NumberAttenuate(float value, float max)
        {
            return Math.Min(value, Math.Max(2, 0.5f * max));
        }

        public static float RangeAttenuate(float value, float range, float distance)
        {
            return Math.Max(0, value * (1f - distance * distance / (range * (range + 1))));
        }

        public static float SignatureAttenuate(float value, float signature)
        {
            return MathF.Sqrt(value * signature);
        }

        public static float VolumeAttenuate(float value)
        {
            return Math.Min(value, s_VolumeConstant * MathF.Sqrt(value));
        }
    }
}

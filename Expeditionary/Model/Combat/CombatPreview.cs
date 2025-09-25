namespace Expeditionary.Model.Combat
{
    public record class CombatPreview
    {
        public record struct Layer(float Attack, float Defense, float DefenseMin, float Probability);

        public static readonly Layer Ignore = new(0f, 0f, 0f, 1f);

        public CombatCondition Condition { get; init; }
        public float Volume { get; init; }
        public float Saturation { get; init; }
        public Layer Target { get; init; }
        public Layer Hit { get; init; }
        public Layer Penetrate { get; init; }
        public Layer Kill { get; init; }
        public float Result { get; init; }

        public CombatPreview() { }

        public CombatPreview(
            CombatCondition condition, 
            float volume,
            float saturation,
            Layer target,
            Layer hit,
            Layer penetrate,
            Layer kill)
        {
            Condition = condition;
            Volume = volume;
            Saturation = saturation;
            Target = target;
            Hit = hit;
            Penetrate = penetrate;
            Kill = kill;
            Result =
                volume 
                * saturation 
                * target.Probability 
                * hit.Probability 
                * penetrate.Probability 
                * kill.Probability;
        }
    }
}

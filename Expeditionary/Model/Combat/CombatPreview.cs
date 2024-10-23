namespace Expeditionary.Model.Combat
{
    public record class CombatPreview
    {
        public record struct Layer(float Attack, float Defense, float DefenseMin, float Probability);

        public float Volume { get; init; }
        public Layer Target { get; init; }
        public Layer Hit { get; init; }
        public Layer Penetrate { get; init; }
        public Layer Kill { get; init; }
        public float Result { get; init; }

        public CombatPreview() { }

        public CombatPreview(float volume, Layer target, Layer hit, Layer penetrate, Layer kill, float result)
        {
            Volume = volume;
            Target = target;
            Hit = hit;
            Penetrate = penetrate;
            Kill = kill;
            Result = result;
        }
    }
}

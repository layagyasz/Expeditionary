namespace Expeditionary.Model.Combat
{
    public record class CombatPreview(
        CombatCondition Condition,
        float Volume,
        float Saturation, 
        CombatPreview.Layer Target,
        CombatPreview.Layer Hit, 
        CombatPreview.Layer Penetrate,
        CombatPreview.Layer Kill, 
        float Diffusion)
    {
        public record struct Layer(float Attack, float Defense, float DefenseMin, float Probability);

        public static readonly Layer Ignore = new(0f, 0f, 0f, 1f);

        public float Result => 
            Volume * Saturation * Target.Probability * Hit.Probability 
            * Penetrate.Probability * Kill.Probability * Diffusion;
    }
}

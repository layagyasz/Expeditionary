namespace Expeditionary.Model.Missions.Objectives
{
    public record struct ObjectiveProgress(float Progress, float Target)
    {
        public float GetPercentDone()
        {
            return Progress / Target;
        }

        public bool IsDone()
        {
            return Progress >= Target;
        }
    }
}

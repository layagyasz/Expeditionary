namespace Expeditionary.Model.Formations
{
    public record class FormationDiad<T>(FormationRole Role, T Unit, T? Transport)
    {
        public IEnumerable<T> GetUnits()
        {
            if (Transport != null)
            {
                yield return Transport;
            }
            yield return Unit;
        }
    }
}

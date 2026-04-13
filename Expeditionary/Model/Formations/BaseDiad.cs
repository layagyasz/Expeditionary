namespace Expeditionary.Model.Formations
{
    public abstract record class BaseDiad<T>(FormationRole Role, T Unit, T? Transport)
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

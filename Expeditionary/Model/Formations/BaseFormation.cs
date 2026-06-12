namespace Expeditionary.Model.Formations
{
    public abstract class BaseFormation<TFormation, TUnit> where TFormation : BaseFormation<TFormation, TUnit>
    {
        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }

        public IEnumerable<FormationDiad<TUnit>> Diads => _diads;
        public IEnumerable<TFormation> ComponentFormations => _componentFormations;

        private readonly List<FormationDiad<TUnit>> _diads;
        private readonly List<TFormation> _componentFormations;

        protected BaseFormation(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<TFormation> componentFormations,
            IEnumerable<FormationDiad<TUnit>> diads)
        {
            Name = name;
            Role = role;
            Echelon = echelon;
            _componentFormations = componentFormations.ToList();
            _diads = diads.ToList();
        }

        public void Add(TFormation component)
        {
            _componentFormations.Add(component);
        }

        public IEnumerable<TFormation> GetComponentFormationsBelow(int echelon)
        {
            if (Echelon <= echelon)
            {
                return Enumerable.Repeat((TFormation) this, 1);
            }
            else
            {
                return _componentFormations.SelectMany(component => component.GetComponentFormationsBelow(echelon));
            }
        }

        public IEnumerable<FormationDiad<TUnit>> GetDiadsAbove(int echelon)
        {
            if (Echelon > echelon)
            {
                foreach (var diad in _diads)
                {
                    yield return diad;
                }
                foreach (var diad in _componentFormations.SelectMany(component => component.GetDiadsAbove(echelon)))
                {
                    yield return diad;
                }
            }
        }

        public IEnumerable<FormationDiad<TUnit>> GetDiads()
        {
            return Enumerable.Concat(Diads, ComponentFormations.SelectMany(x => x.GetDiads()));
        }

        public IEnumerable<TUnit> GetUnits()
        {
            return _diads.SelectMany(diad => diad.GetUnits()).Concat(
                _componentFormations.SelectMany(component => component.GetUnits()));
        }
    }
}

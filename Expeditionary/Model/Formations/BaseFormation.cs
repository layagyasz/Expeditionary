namespace Expeditionary.Model.Formations
{
    public abstract class BaseFormation<TFormation, TDiad, TUnit> 
        where TFormation : BaseFormation<TFormation, TDiad, TUnit>
        where TDiad : BaseDiad<TUnit>
    {
        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }

        public IEnumerable<TDiad> Diads => _diads;
        public IEnumerable<TFormation> ComponentFormations => _componentFormations;

        protected readonly List<TDiad> _diads;
        protected readonly List<TFormation> _componentFormations;

        protected BaseFormation(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<TFormation> componentFormations,
            IEnumerable<TDiad> diads)
        {
            Name = name;
            Role = role;
            Echelon = echelon;
            _componentFormations = componentFormations.ToList();
            _diads = diads.ToList();
        }

        public IEnumerable<TDiad> GetDiads()
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

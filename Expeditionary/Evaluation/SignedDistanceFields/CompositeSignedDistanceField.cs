using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.SignedDistanceFields
{
    public class CompositeSignedDistanceField : ISignedDistanceField
    {
        public IEnumerable<ISignedDistanceField> Components => _components;
        public int MaxInternalDistance { get; private set; }
        public int MaxExternalDistance { get; private set; }

        private readonly List<ISignedDistanceField> _components;

        public CompositeSignedDistanceField(IEnumerable<ISignedDistanceField> components)
        {
            _components = components.ToList();
            MaxInternalDistance = _components.Select(x => x.MaxInternalDistance).Max();
            MaxExternalDistance = _components.Select(x => x.MaxExternalDistance).Max();
        }

        public CompositeSignedDistanceField()
        {
            _components = new();
        }

        public void Add(ISignedDistanceField component)
        {
            _components.Add(component);
            MaxInternalDistance = Math.Max(MaxInternalDistance, component.MaxInternalDistance);
            MaxExternalDistance = Math.Max(MaxExternalDistance, component.MaxExternalDistance);
        }

        public int Get(Vector3i hex)
        {
            if (!_components.Any())
            {
                return 0;
            }
            return _components.Min(x => x.Get(hex));
        }
    }
}

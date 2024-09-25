using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public class Match
    {
        public EventHandler<Unit>? AssetAdded { get; set; }
        public EventHandler<Unit>? AssetRemoved { get; set; }

        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;

        private readonly List<Unit> _units = new();

        public Match(IIdGenerator idGenerator, Map map)
        {
            _idGenerator = idGenerator;
            _map = map;
        }

        public void Add(UnitType unitType, Vector3i position)
        {
            var asset = new Unit(_idGenerator.Next(), unitType) {  Position = position };
            _units.Add(asset);
            AssetAdded?.Invoke(this, asset);
        }

        public Map GetMap()
        {
            return _map;
        }

        public IEnumerable<Unit> GetAssets()
        {
            return _units;
        }
    }
}

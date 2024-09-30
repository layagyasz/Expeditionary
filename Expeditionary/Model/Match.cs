using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public class Match
    {
        public EventHandler<IAsset>? AssetAdded { get; set; }
        public EventHandler<IAsset>? AssetRemoved { get; set; }

        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;

        private readonly List<IAsset> _assets = new();

        public Match(IIdGenerator idGenerator, Map map)
        {
            _idGenerator = idGenerator;
            _map = map;
        }

        public void Add(UnitType unitType, Vector3i position)
        {
            var asset = new Unit(_idGenerator.Next(), unitType) {  Position = position };
            _assets.Add(asset);
            AssetAdded?.Invoke(this, asset);
        }

        public Map GetMap()
        {
            return _map;
        }

        public IEnumerable<IAsset> GetAssets()
        {
            return _assets;
        }

        public void Remove(IAsset asset)
        {
            _assets.Remove(asset);
            AssetRemoved?.Invoke(this, asset);
        }
    }
}

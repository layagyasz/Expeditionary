using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public class Match
    {
        public EventHandler<IAsset>? AssetAdded { get; set; }
        public EventHandler<AssetMovedEventArgs>? AssetMoved { get; set; }
        public EventHandler<IAsset>? AssetRemoved { get; set; }

        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;

        private readonly Dictionary<Player, PlayerKnowledge> _playerKnowledge;
        private readonly List<IAsset> _assets = new();

        public Match(IIdGenerator idGenerator, Map map, Dictionary<Player, PlayerKnowledge> playerKnowledge)
        {
            _idGenerator = idGenerator;
            _map = map;
            _playerKnowledge = playerKnowledge;
        }

        public void Add(UnitType unitType, Player player, Vector3i position)
        {
            var asset = new Unit(_idGenerator.Next(), player, unitType) {  Position = position };
            _assets.Add(asset);
            AssetAdded?.Invoke(this, asset);
        }

        public PlayerKnowledge GetKnowledge(Player player)
        {
            return _playerKnowledge[player];
        }

        public Map GetMap()
        {
            return _map;
        }

        public IEnumerable<IAsset> GetAssets()
        {
            return _assets;
        }

        public void Move(IAsset asset, Pathing.Path path)
        {
            asset.Position = path.Destination;
            AssetMoved?.Invoke(this, new(asset, path.Origin, path.Destination, path));
        }

        public void Remove(IAsset asset)
        {
            _assets.Remove(asset);
            AssetRemoved?.Invoke(this, asset);
        }

        public void Reset()
        {
            _assets.ForEach(x => x.Reset());
        }
    }
}

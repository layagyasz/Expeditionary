using Cardamom.Collections;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public class Match
    {
        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;

        private readonly Dictionary<Player, PlayerKnowledge> _playerKnowledge;
        private readonly List<IAsset> _assets = new();
        private readonly MultiMap<Vector3i, IAsset> _positions = new();

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
            _positions.Add(position, asset);

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Add(asset, position, _positions);
            }
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

        public IEnumerable<IAsset> GetAssetsAt(Vector3i hex)
        {
            return _positions[hex];
        }

        public void Initialize()
        {
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.AssetKnowledgeChanged += HandleAssetKnowledgeChanged;
                knowledge.MapKnowledgeChanged += HandleMapKnowledgeChanged;
            }
        }

        public void Move(IAsset asset, Pathing.Path path)
        {
            asset.Position = path.Destination;
            _positions.Remove(path.Origin, asset);
            _positions.Add(path.Destination, asset);

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Move(asset, path, _positions);
            }
        }

        public void Remove(IAsset asset)
        {
            _assets.Remove(asset);
            _positions.Remove(asset.Position, asset);

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Remove(asset, _positions);
            }
        }

        public void Reset()
        {
            _assets.ForEach(x => x.Reset());
        }

        private void HandleAssetKnowledgeChanged(object? sender, AssetKnowledgeChangedEventArgs e)
        {
            AssetKnowledgeChanged?.Invoke(this, e);
        }

        private void HandleMapKnowledgeChanged(object? sender, MapKnowledgeChangedEventArgs e)
        {
            MapKnowledgeChanged?.Invoke(this, e);
        }
    }
}

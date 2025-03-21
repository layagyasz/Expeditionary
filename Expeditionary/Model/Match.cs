using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public class Match
    {
        private static readonly ILogger s_Logger = new Logger(new ConsoleBackend(), LogLevel.Info);

        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }
        public EventHandler<IOrder>? OrderAdded { get; set; }
        public EventHandler<IOrder>? OrderRemoved { get; set; }
        public EventHandler<EventArgs>? Stepped { get; set; }

        private readonly Random _random;
        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;

        private readonly List<Player> _players = new();
        private readonly Dictionary<Player, ObjectiveSet> _playerObjectives = new();
        private readonly Dictionary<Player, PlayerStatistics> _playerStatistics = new();
        private readonly Dictionary<Player, IPlayerKnowledge> _playerKnowledge = new();
        private readonly List<IAsset> _assets = new();
        private readonly MultiMap<Vector3i, IAsset> _positions = new();

        private int _activePlayer = -1;

        public Match(Random random, IIdGenerator idGenerator, Map map)
        {
            _random = random;
            _idGenerator = idGenerator;
            _map = map;
        }

        public void Add(Player player, ObjectiveSet objectives, IPlayerKnowledge knowledge)
        {
            _players.Add(player);
            _playerObjectives.Add(player, objectives);
            _playerStatistics.Add(player, new());
            _playerKnowledge.Add(player, knowledge);
            foreach (var asset in _assets)
            {
                knowledge.Add(asset, asset.Position, _positions);
            }
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

        public void Damage(Unit attacker, Unit defender, int kills)
        {
            defender.Damage(kills);
            if (defender.Number <= 0)
            {
                Destroy(defender);

                var attackerStats = _playerStatistics[attacker.Player];
                attackerStats.Destroyed += defender.UnitQuantity;

                var defenderStats = _playerStatistics[defender.Player];
                defenderStats.Lost += defender.UnitQuantity;
            }
        }

        public void Destroy(Unit unit)
        {
            unit.Destroy();
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Destroy(unit, _positions);
            }
        }

        public bool DoOrder(IOrder order)
        {
            if (!ValidatePlayer(order.Unit.Player))
            {
                s_Logger.Log($"{order} failed player validation");
                return false;
            }
            if (!order.Validate(this))
            {
                s_Logger.Log($"{order} failed match validation");
                return false;
            }
            s_Logger.Log($"{order} executed");
            order.Execute(this);
            return true;
        }

        public IPlayerKnowledge GetKnowledge(Player player)
        {
            return _playerKnowledge[player];
        }

        public Map GetMap()
        {
            return _map;
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }

        public IEnumerable<IAsset> GetAssets()
        {
            return _assets;
        }

        public IEnumerable<IAsset> GetAssetsAt(Vector3i hex)
        {
            return _positions[hex];
        }

        public IEnumerable<IAsset> GetAssetsIn(MapTag tag)
        {
            return _map.GetArea(tag).SelectMany(x => _positions[x]);
        }

        public IEnumerable<ObjectiveSet> GetObjectiveSets(int team)
        {
            return _playerObjectives.Where(x => x.Key.Team == team).Select(x => x.Value);
        }

        public Random GetRandom()
        {
            return _random;
        }

        public PlayerStatistics GetStatistics(Player player)
        {
            return _playerStatistics[player];
        }

        public IEnumerable<PlayerStatistics> GetStatistics(int team)
        {
            return _playerStatistics.Where(x => x.Key.Team == team).Select(x => x.Value);
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

        public void Step()
        {
            _activePlayer++;
            if (_activePlayer >= GetPlayers().Count())
            {
                _activePlayer = 0;
                Reset();
            }
            Stepped?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidatePlayer(Player player)
        {
            return player.Id == _activePlayer;
        }

        private void HandleAssetKnowledgeChanged(object? sender, AssetKnowledgeChangedEventArgs e)
        {
            Console.WriteLine($"{e.Player}: {string.Join(",", e.Delta)}");
            AssetKnowledgeChanged?.Invoke(this, e);
        }

        private void HandleMapKnowledgeChanged(object? sender, MapKnowledgeChangedEventArgs e)
        {
            MapKnowledgeChanged?.Invoke(this, e);
        }
    }
}

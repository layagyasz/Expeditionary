using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Caches;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Events;
using Expeditionary.Model.Formations;
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
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(Match));

        public event EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged;
        public event EventHandler<EventArgs>? Finished;
        public event EventHandler<Formation>? FormationAdded;
        public event EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged;
        public event EventHandler<EventArgs>? Stepped;

        private readonly IEventBuffer _events = new SimpleEventBuffer();

        private readonly Random _random;
        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;
        private readonly EvaluationCache _evaluationCache;
        private readonly TileEvaluator _tileEvaluator;

        private readonly List<Player> _players = new();
        private readonly Dictionary<Player, IObjective> _playerObjectives = new();
        private readonly Dictionary<Player, PlayerStatistics> _playerStatistics = new();
        private readonly Dictionary<Player, IPlayerKnowledge> _playerKnowledge = new();
        private readonly List<Formation> _formations = new();
        private readonly List<IAsset> _assets = new();
        private readonly MultiMap<Vector3i, IAsset> _positions = new();

        private int _turn = 0;
        private int _activePlayer = -1;

        public Match(Random random, IIdGenerator idGenerator, Map map)
        {
            _random = random;
            _idGenerator = idGenerator;
            _map = map;
            _evaluationCache = new EvaluationCache(new ExposureCache(_map), new PartitionCache(_map));
            _tileEvaluator = new TileEvaluator(_evaluationCache, random);
        }

        public void Add(Player player, IObjective objective, IPlayerKnowledge knowledge)
        {
            _players.Add(player);
            _playerObjectives.Add(player, objective);
            _playerStatistics.Add(player, new());
            _playerKnowledge.Add(player, knowledge);
            foreach (var asset in _assets)
            {
                if (asset.Position.HasValue)
                {
                    knowledge.Place(asset, asset.Position.Value, _positions);
                }
            }
            s_Logger.Log($"{player} added");
        }

        public Formation Add(Player player, FormationTemplate template)
        {
            var formation = template.Materialize(player, _idGenerator);
            _formations.Add(formation);
            s_Logger.Log($"{formation} added for {player} with strength {formation.GetAliveUnitQuantity()}");
            _events.Queue(FormationAdded, this, formation);
            return formation;
        }

        public void Damage(Unit attacker, Unit defender, int kills)
        {
            s_Logger.Log($"{attacker} damaged {defender} by {kills}");
            defender.Damage(kills);
            if (defender.Number <= 0)
            {
                Destroy(defender);

                var attackerStats = _playerStatistics[attacker.Player];
                attackerStats.Destroyed += defender.Value;

                var defenderStats = _playerStatistics[defender.Player];
                defenderStats.Lost += defender.Value;
            }
        }

        public void Destroy(Unit unit)
        {
            s_Logger.Log($"{unit} destroyed");
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Destroy(unit, _positions);
            }
            unit.Destroy();
            if (unit.Passenger != null && unit.Passenger is Unit passenger)
            {
                Destroy(passenger);
            }
        }

        public void DispatchEvents(long delta)
        {
            _events.DispatchEvents(delta);
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

        public Player GetActivePlayer()
        {
            return _players[_activePlayer];
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

        public TileEvaluator GetEvaluator()
        {
            return _tileEvaluator;
        }

        public UnitTileEvaluator GetEvaluatorFor(Unit unit, MapDirection facing)
        {
            return new UnitTileEvaluator(
                unit,
                facing,
                RangeBucketizer.ToBucket(unit.Type), 
                GetKnowledge(unit.Player), 
                _evaluationCache, 
                _random);
        }

        public IEnumerable<Formation> GetFormations(Player player)
        {
            return _formations.Where(x => x.Player.Id == player.Id);
        }

        public IPlayerKnowledge GetKnowledge(Player player)
        {
            return _playerKnowledge[player];
        }

        public Map GetMap()
        {
            return _map;
        }

        public IObjective GetObjective(Player player)
        {
            return _playerObjectives[player];
        }

        public IEnumerable<(Player, IObjective)> GetObjectives(int team)
        {
            return _playerObjectives.Where(x => x.Key.Team == team).Select(kvp => (kvp.Key, kvp.Value));
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }

        public Random GetRandom()
        {
            return _random;
        }

        public PlayerStatistics GetStatistics(Player player)
        {
            return _playerStatistics[player];
        }

        public int GetTurn()
        {
            return _turn;
        }

        public IEnumerable<PlayerStatistics> GetStatistics(int team)
        {
            return _playerStatistics.Where(x => x.Key.Team == team).Select(x => x.Value);
        }

        public void Initialize()
        {
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.AssetKnowledgeChanged += 
                    _events.Hook<AssetKnowledgeChangedEventArgs>(HandleAssetKnowledgeChanged);
                knowledge.MapKnowledgeChanged += 
                    _events.Hook<MapKnowledgeChangedEventArgs>(HandleMapKnowledgeChanged);
            }
            s_Logger.Log("Initialized");
        }

        public void Load(Unit unit, IAsset asset)
        {
            s_Logger.Log($"{unit} loaded {asset}");
            unit.Passenger = asset;
            asset.IsPassenger = true;

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Suppress(asset, _positions);
            }
        }

        public void Move(IAsset asset, Pathing.Path path)
        {
            s_Logger.Log($"{asset} moved along {path}");
            asset.Position = path.Destination;
            _positions.Remove(path.Origin, asset);
            _positions.Add(path.Destination, asset);

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Move(asset, path, _positions);
            }

            if (asset is Unit unit && unit.Passenger != null)
            {
                Move(unit.Passenger, path);
            }
        }

        public void Place(IAsset asset, Vector3i position)
        {
            s_Logger.Log($"{asset} placed at {position}");
            if (asset.Position.HasValue)
            {
                _positions.Remove(asset.Position.Value, asset);
                
                foreach (var knowledge in _playerKnowledge.Values)
                {
                    knowledge.Remove(asset, _positions);
                }
            }
            else
            {
                _assets.Add(asset);
            }
            _positions.Add(position, asset);
            asset.Position = position;

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Place(asset, position, _positions);
            }
        }

        public void Remove(IAsset asset)
        {
            s_Logger.Log($"{asset} removed");
            _assets.Remove(asset);
            if (asset.Position.HasValue)
            {
                _positions.Remove(asset.Position.Value, asset);
                asset.Position = null;
            }

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Remove(asset, _positions);
            }
        }

        public void Reset()
        {
            s_Logger.Log("Reset assets");
            _assets.ForEach(x => x.Reset());
        }

        public void Step()
        {
            if (_playerObjectives.Any(entry => entry.Value.Evaluate(entry.Key, this).IsTerminal))
            {
                _events.Queue(Finished, this, EventArgs.Empty);
                return;
            }
            _activePlayer++;
            if (_activePlayer >= GetPlayers().Count())
            {
                _activePlayer = 0;
                _turn++;
                Reset();
            }
            s_Logger.Log($"Entered turn {_players[_activePlayer]}");
            _events.Queue(Stepped, this, EventArgs.Empty);
        }

        public void Unload(Unit unit)
        {
            s_Logger.Log($"{unit} unloaded");
            var passenger = unit.Passenger;
            if (passenger != null)
            {
                passenger.IsPassenger = false;

                if (passenger.Position != null)
                {
                    foreach (var knowledge in _playerKnowledge.Values)
                    {
                        knowledge.Place(passenger, passenger.Position.Value, _positions);
                    }
                }
            }
            unit.Passenger = null;
        }

        private bool ValidatePlayer(Player player)
        {
            return player.Id == _activePlayer;
        }

        private void HandleAssetKnowledgeChanged(object? sender, AssetKnowledgeChangedEventArgs e)
        {
            s_Logger.Log(e.ToString());
            AssetKnowledgeChanged?.Invoke(sender, e);
        }

        private void HandleMapKnowledgeChanged(object? sender, MapKnowledgeChangedEventArgs e)
        {
            s_Logger.Log(e.ToString());
            MapKnowledgeChanged?.Invoke(sender, e);
        }
    }
}

using Cardamom;
using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Events;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Evaluation;
using Expeditionary.Model.Matches.Evaluation.Caches;
using Expeditionary.Model.Matches.Evaluation.TileEvaluators;
using Expeditionary.Model.Matches.Events;
using Expeditionary.Model.Matches.Knowledge;
using Expeditionary.Model.Matches.Orders;
using Expeditionary.Model.Matches.Reporting;
using Expeditionary.Model.Missions.Objectives;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches
{
    public class Match
    {
        private static readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(Match));

        public event EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged;
        public event EventHandler<EventArgs>? Finished;
        public event EventHandler<FormationAddedEventArgs>? FormationAdded;
        public event EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged;
        public event EventHandler<EventArgs>? Stepped;

        private readonly IEventBuffer _eventBuffer = new SimpleEventBuffer();

        private readonly Random _random;
        private readonly IIdGenerator _idGenerator;
        private readonly Map _map;
        private readonly EvaluationCache _evaluationCache;
        private readonly TileEvaluator _tileEvaluator;

        private readonly List<MatchPlayer> _players = new();
        private readonly Dictionary<MatchPlayer, IObjective> _playerObjectives = new();
        private readonly Dictionary<MatchPlayer, PlayerStatistics> _playerReports = new();
        private readonly Dictionary<MatchPlayer, IPlayerKnowledge> _playerKnowledge = new();
        private readonly List<MatchFormation> _formations = new();
        private readonly List<IMatchAsset> _assets = new();
        private readonly MultiMap<Vector3i, IMatchAsset> _positions = new();
        private readonly List<IEvent> _events = new();

        private int _turn = 0;
        private int _activePlayer = -1;

        public TurnInfo CurrentTurn => new(_turn, GetPlayer(_activePlayer), TurnSegment.Active);

        public Match(Random random, IIdGenerator idGenerator, Map map)
        {
            _random = random;
            _idGenerator = idGenerator;
            _map = map;
            _evaluationCache = new EvaluationCache(new ExposureCache(_map), new PartitionCache(_map));
            _tileEvaluator = new TileEvaluator(_evaluationCache, random);
        }

        public void Add(MatchPlayer player, IObjective objective, IPlayerKnowledge knowledge)
        {
            _players.Add(player);
            _playerObjectives.Add(player, objective);
            _playerReports.Add(player, new());
            _playerKnowledge.Add(player, knowledge);
            foreach (var asset in _assets)
            {
                if (asset.IsActive)
                {
                    knowledge.Place(asset, asset.Position, _positions);
                }
            }
            s_Logger.Log($"{player} added");
        }

        public MatchFormation Add(MatchPlayer player, InstanceFormation instance, MatchFormation? parent = null)
        {
            return Add(player, MatchFormation.From(instance, player, _idGenerator), parent);
        }

        public void Add(IEvent @event)
        {
            _events.Add(@event);
        }

        public void Damage(MatchUnit attacker, MatchUnit defender, int kills)
        {
            s_Logger.Log($"{attacker} damaged {defender} by {kills}");
            defender.Damage(kills);
            if (defender.Number <= 0)
            {
                Destroy(defender);

                var attackerReport = _playerReports[attacker.Player];
                attackerReport.Destroyed += defender.Value;

                var defenderReport = _playerReports[defender.Player];
                defenderReport.Lost += defender.Value;
            }
        }

        public void Destroy(MatchUnit unit)
        {
            s_Logger.Log($"{unit} destroyed");
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Destroy(unit, _positions);
            }
            RemoveInternal(unit);
            unit.Status = MatchAssetStatus.Destroyed;
            if (unit.Passenger != null && unit.Passenger is MatchUnit passenger)
            {
                Destroy(passenger);
            }
        }

        public void DispatchEvents(long delta)
        {
            _eventBuffer.DispatchEvents(delta);
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

        public void Evacuate(IMatchAsset asset)
        {
            RemoveInternal(asset);
            asset.Status = MatchAssetStatus.Evacuated;
        }

        public IEnumerable<IMatchAsset> GetAssets()
        {
            return _assets;
        }

        public IEnumerable<IMatchAsset> GetAssetsAt(Vector3i hex)
        {
            return _positions[hex];
        }

        public IEnumerable<IMatchAsset> GetAssetsIn(MapTag tag)
        {
            return _map.GetArea(tag).SelectMany(x => _positions[x]);
        }

        public TileEvaluator GetEvaluator()
        {
            return _tileEvaluator;
        }

        public UnitTileEvaluator GetEvaluatorFor(MatchUnit unit, MapDirection facing)
        {
            return new UnitTileEvaluator(
                unit,
                facing,
                RangeBucketizer.ToBucket(unit.Type),
                GetKnowledge(unit.Player),
                _evaluationCache,
                _random);
        }

        public IEnumerable<MatchFormation> GetFormations(MatchPlayer player)
        {
            return _formations.Where(x => x.Player.Id == player.Id);
        }

        public IPlayerKnowledge GetKnowledge(MatchPlayer player)
        {
            return _playerKnowledge[player];
        }

        public Map GetMap()
        {
            return _map;
        }

        public IObjective GetObjective(MatchPlayer player)
        {
            return _playerObjectives[player];
        }

        public ObjectiveStatus GetObjectiveStatus(MatchPlayer player)
        {
            return GetObjective(player).Evaluate(player, this).Status;
        }

        public IEnumerable<(MatchPlayer, IObjective)> GetObjectives(int team)
        {
            return _playerObjectives.Where(x => x.Key.Team == team).Select(kvp => (kvp.Key, kvp.Value));
        }

        public IEnumerable<MatchPlayer> GetPlayers()
        {
            return _players;
        }

        public Random GetRandom()
        {
            return _random;
        }

        public PlayerStatistics GetStatistics(MatchPlayer player)
        {
            return _playerReports[player];
        }

        public IEnumerable<PlayerStatistics> GetStatistics(int team)
        {
            return _playerReports.Where(x => x.Key.Team == team).Select(x => x.Value);
        }

        public void Initialize()
        {
            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.AssetKnowledgeChanged +=
                    _eventBuffer.Hook<AssetKnowledgeChangedEventArgs>(HandleAssetKnowledgeChanged);
                knowledge.MapKnowledgeChanged +=
                    _eventBuffer.Hook<MapKnowledgeChangedEventArgs>(HandleMapKnowledgeChanged);
            }
            s_Logger.Log("Initialized");
        }

        public void Load(MatchUnit unit, IMatchAsset asset)
        {
            s_Logger.Log($"{unit} loaded {asset}");
            unit.Passenger = asset;
            asset.IsPassenger = true;

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Suppress(asset, _positions);
            }
        }

        public void Move(IMatchAsset asset, Pathing.Path path)
        {
            s_Logger.Log($"{asset} moved along {path}");
            asset.Position = path.Destination;
            _positions.Remove(path.Origin, asset);
            _positions.Add(path.Destination, asset);

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Move(asset, path, _positions);
            }

            if (asset is MatchUnit unit && unit.Passenger != null)
            {
                Move(unit.Passenger, path);
            }
        }

        public void Place(IMatchAsset asset, Vector3i position)
        {
            s_Logger.Log($"{asset} placed at {position}");
            if (asset.IsActive)
            {
                _positions.Remove(asset.Position, asset);

                foreach (var knowledge in _playerKnowledge.Values)
                {
                    knowledge.Remove(asset, _positions);
                }
            }
            _positions.Add(position, asset);
            asset.Position = position;

            foreach (var knowledge in _playerKnowledge.Values)
            {
                knowledge.Place(asset, position, _positions);
            }

            asset.Status = MatchAssetStatus.Active;
        }

        public void Remove(IMatchAsset asset)
        {
            RemoveInternal(asset);
            asset.Status = MatchAssetStatus.Reserved;
        }

        public void Reset()
        {
            s_Logger.Log("Reset assets");
            _assets.ForEach(x => x.Reset());
        }

        public void Step()
        {
            DoEvents(new(_turn, GetPlayer(_activePlayer), _activePlayer >= 0 ? TurnSegment.End : TurnSegment.Start));
            _activePlayer++;
            if (_activePlayer >= GetPlayers().Count())
            {
                DoEvents(new(_turn, GetPlayer(_activePlayer), TurnSegment.End));
                _turn++;
                Reset();
                DoEvents(new(_turn, GetPlayer(_activePlayer), TurnSegment.Start));
                _activePlayer = 0;
            }
            if (_playerObjectives.Any(entry => entry.Value.Evaluate(entry.Key, this).IsTerminal))
            {
                _eventBuffer.Queue(Finished, this, EventArgs.Empty);
                return;
            }
            DoEvents(new(_turn, GetPlayer(_activePlayer), TurnSegment.Start));
            s_Logger.Log($"Entered turn {_players[_activePlayer]} {_turn}");
            _eventBuffer.Queue(Stepped, this, EventArgs.Empty);
        }

        public void Unload(MatchUnit unit)
        {
            s_Logger.Log($"{unit} unloaded");
            var passenger = unit.Passenger;
            if (passenger != null)
            {
                passenger.IsPassenger = false;
                foreach (var knowledge in _playerKnowledge.Values)
                {
                    knowledge.Place(passenger, passenger.Position, _positions);
                }
            }
            unit.Passenger = null;
        }

        private MatchFormation Add(MatchPlayer player, MatchFormation formation, MatchFormation? parent)
        {
            Precondition.Check(parent == null || parent.Player == player);
            if (parent == null)
            {
                _formations.Add(formation);
            }
            else
            {
                parent.AddComponent(formation);
            }
            foreach (var unit in formation.GetUnits())
            {
                _assets.Add(unit);
            }
            s_Logger.Log($"{formation} added for {player} with strength {formation.GetAliveUnitQuantity()}");
            _eventBuffer.Queue(FormationAdded, this, new(formation, parent));
            return formation;
        }

        private void DoEvents(TurnInfo turn)
        {
            var removedEvents = new List<IEvent>();
            foreach (var @event in _events)
            {
                if (@event.Fire(this, turn) == EventStatus.Done)
                {
                    removedEvents.Add(@event);
                }
            }
            if (removedEvents.Any())
            {
                _events.RemoveAll(removedEvents.Contains);
            }
        }

        private MatchPlayer? GetPlayer(int playerId)
        {
            return playerId >= 0 ? _players[playerId] : null;
        }

        private void RemoveInternal(IMatchAsset asset)
        {
            s_Logger.Log($"{asset} removed");
            if (asset.IsActive)
            {
                _positions.Remove(asset.Position, asset);
                asset.Position = default;
                foreach (var knowledge in _playerKnowledge.Values)
                {
                    knowledge.Remove(asset, _positions);
                }
            }
        }

        private bool ValidatePlayer(MatchPlayer player)
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

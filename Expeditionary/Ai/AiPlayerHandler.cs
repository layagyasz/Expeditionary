using Cardamom;
using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class AiPlayerHandler : IDisposable, IInitializable
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiPlayerHandler));

        public Player Player { get; }

        private readonly Match _match;
        private readonly EvaluationCache _evaluationCache;
        private readonly Random _random;
        private readonly IPlayerKnowledge _knowledge;
        private readonly RootFormationHandler _rootFormationHandler;

        public AiPlayerHandler(Player player, Match match, EvaluationCache evaluationCache, Random random)
        {
            Player = player;
            _match = match;
            _evaluationCache = evaluationCache;
            _random = random;
            _knowledge = match.GetKnowledge(Player);
            _rootFormationHandler = 
                new RootFormationHandler(player, match.GetFormations(Player).Select(SimpleFormationHandler.Create));
        }

        public IFormationHandler AddFormation(Formation formation)
        {
            var result = SimpleFormationHandler.Create(formation);
            _rootFormationHandler.Add(result);
            return result;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _match.FormationAdded -= HandleFormationAdded;
            s_Logger.With(Player.Id).Log("Disposed");
        }

        public void DoTurn()
        {
            s_Logger.With(Player.Id).Log($"started automated turn");
            foreach (var formation in _rootFormationHandler.GetAllFormationHandlers())
            {
                DoFormationTurn(formation);
            }
            Thread.Sleep(1000);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        public void Initialize()
        {
            _match.FormationAdded += HandleFormationAdded;
            s_Logger.With(Player.Id).Log("Initialized");
        }

        public void Setup()
        {
            s_Logger.With(Player.Id).Log("Setup formations");
            foreach (var formation in _rootFormationHandler.Children)
            {
                formation.Reevaluate(_match, _evaluationCache, _random);
            }
            s_Logger.With(Player.Id).Log("Setup units");
            foreach (var unit in _rootFormationHandler.GetAllUnitHandlers())
            {
                unit.Assignment.Place(unit, _match);
            }
        }

        private void DoFormationTurn(SimpleFormationHandler formation)
        {
            foreach (var unit in formation.GetUnitHandlers())
            {
                DoUnitTurn(unit);
            }
        }

        private void DoUnitTurn(UnitHandler unit)
        {
            var map = _match.GetMap();
            var attack = unit.Unit.Type.Weapons.First();
            var mode = attack.Weapon!.Modes.First();
            var target = 
                FindValidTargets(unit.Unit, mode, map)
                    .Select(target => (target, AttackEvaluation.Evaluate(unit.Unit, attack, mode, target, map)))
                    .ArgMax(x => x.Item2);
            if (target.target != null)
            {
                s_Logger.With(Player.Id).Log($"{unit} found target {target.target} with value {target.Item2}");
                _match.DoOrder(new AttackOrder(unit.Unit, attack, mode, target.target));
                Thread.Sleep(1000);
            }
        }

        private IEnumerable<Unit> FindValidTargets(Unit unit, UnitWeapon.Mode mode, Map map)
        {
            return _match.GetAssets()
                .Where(x => !x.IsDestroyed)
                .Where(x => _knowledge.GetAsset(x).IsVisible)
                .Where(x => x is Unit)
                .Cast<Unit>()
                .Where(x => CombatCalculator.IsValidTarget(unit, mode, x, map));
        }

        private void HandleFormationAdded(object? sender, Formation formation)
        {
            // Add formation in correct location in formation tree.
            s_Logger.With(Player.Id).AtWarning().Log("HandleFormationAdded not implemented");
        }
    }
}

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
        private readonly List<FormationHandler> _formations;

        public AiPlayerHandler(Player player, Match match, EvaluationCache evaluationCache, Random random)
        {
            Player = player;
            _match = match;
            _evaluationCache = evaluationCache;
            _random = random;
            _knowledge = match.GetKnowledge(Player);
            _formations = match.GetFormations(Player).Select(FormationHandler.Create).ToList();
        }

        public FormationHandler AddFormation(Formation formation)
        {
            var result = FormationHandler.Create(formation);
            _formations.Add(result);
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
            _formations.ForEach(DoFormationTurn);
            Thread.Sleep(1000);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        public IEnumerable<FormationHandler> GetFormationAssignments()
        {
            return _formations;
        }

        public IEnumerable<UnitHandler> GetUnitHandlers()
        {
            return _formations.SelectMany(x => x.GetUnitHandlers());
        }

        public void Initialize()
        {
            _match.FormationAdded += HandleFormationAdded;
            s_Logger.With(Player.Id).Log("Initialized");
        }

        public void Setup()
        {
            s_Logger.With(Player.Id).Log("Setup");
            foreach (var formation in GetFormationAssignments())
            {
                formation.AssignChildren(_match, _evaluationCache, _random);
            }
            foreach (var unit in GetUnitHandlers())
            {
                unit.Assignment.Place(unit, _match);
            }
        }

        private void DoFormationTurn(FormationHandler formation)
        {
            foreach (var unit in formation.GetUnitHandlers().Where(x => x.IsActive()))
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

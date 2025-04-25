using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class AiPlayer
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiPlayer));

        public Player Player { get; }

        private readonly Match _match;
        private readonly IPlayerKnowledge _knowledge;
        private readonly List<FormationAssignment> _formations;

        public AiPlayer(Player player, Match match)
        {
            Player = player;
            _match = match;
            _knowledge = match.GetKnowledge(Player);
            _formations = match.GetFormations(Player).Select(FormationAssignment.Create).ToList();
        }

        public void DoTurn()
        {
            s_Logger.With(Player.Id).Log($"started automated turn");
            _formations.ForEach(DoFormationTurn);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        private void DoFormationTurn(FormationAssignment formation)
        {
            foreach (var unit in formation.Units.Where(x => x.IsActive()))
            {
                DoUnitTurn(unit);
            }
        }

        private void DoUnitTurn(UnitAssignment unit)
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
    }
}

using Cardamom.Logging;
using Expeditionary.Evaluation;
using Expeditionary.Model;
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

        public AiPlayer(Player player, Match match)
        {
            Player = player;
            _match = match;
            _knowledge = match.GetKnowledge(Player);
        }

        public void DoTurn()
        {
            s_Logger.With(Player.Id).Log($"started automated turn");
            foreach (var unit in _match.GetAssets().Where(x => x is Unit).Cast<Unit>().Where(x => x.Player == Player))
            {
                DoUnitTurn(unit);
            }
            Thread.Sleep(5000);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        private void DoUnitTurn(Unit unit)
        {
            var map = _match.GetMap();
            var attack = unit.Type.Weapons.First();
            var mode = attack.Weapon!.Modes.First();
            var target = 
                FindValidTargets(unit, mode, map)
                    .Select(target => (target, AttackEvaluation.Evaluate(unit, attack, mode, target, map)))
                    .MaxBy(x => x.Item2);
            if (target.target != null)
            {
                s_Logger.With(Player.Id).Log($"{unit} found target {target.target} with value {target.Item2}");
                // Need to implement event buffers to send orders
                // _match.DoOrder(new AttackOrder(unit, attack, mode, target.target));
            }
        }

        private IEnumerable<Unit> FindValidTargets(Unit unit, UnitWeapon.Mode mode, Map map)
        {
            return _match.GetAssets()
                .Where(x => _knowledge.GetAsset(x).IsVisible)
                .Where(x => x is Unit)
                .Cast<Unit>()
                .Where(x => AttackEvaluation.IsValidTarget(unit, mode, x, map));
        }
    }
}

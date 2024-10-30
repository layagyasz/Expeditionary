using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public Player Player { get; }
        public string TypeKey => Type.Key;
        public Vector3i Position { get; set; }
        public UnitType Type { get; }
        public int Number { get; private set; }
        public float Movement { get; set; }
        public bool Attacked { get; set; }

        private readonly int[] _attackNumbers;


        public Unit(int id, Player player, UnitType type)
        {
            Id = id;
            Player = player;
            Type = type;

            _attackNumbers = type.Weapons.Select(x => x.Number).ToArray();

            Number = (int)type.Intrinsics.Number.GetValue();
            Reset();
        }

        public void Damage(int kills, Random random)
        {
            var limits = new int[_attackNumbers.Length];
            var indices = new int[_attackNumbers.Length];
            var total = 0;
            int a = 0;
            for (int i=0; i<Type.Weapons.Count; ++i)
            {
                var attack = Type.Weapons[i];
                var count = _attackNumbers[i];
                if (attack.IsDistributed)
                {
                    _attackNumbers[i] -= kills;
                }
                else
                {
                    total += count;
                    limits[a] = total;
                    indices[a++] = i;
                }
            }
            if (a > 0)
            {
                for (int i=0; i<kills; ++i)
                {
                    var draw = random.Next(total);
                    var index = Enumerable.Range(0, limits.Length).Where(x => limits[x] > draw).Last();
                    _attackNumbers[indices[index]] -= 1;
                }
            }
            Number -= kills;
        }

        public int GetAttackNumber(UnitWeaponDistribution attack)
        {
            return _attackNumbers[Type.Weapons.IndexOf(attack)];
        }

        public void Reset()
        {
            Movement = Type.Speed;
            Attacked = false;
        }
    }
}

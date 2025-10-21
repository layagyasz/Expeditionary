using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Units;

namespace Expeditionary.Runners.Recorders
{
    public static class BalanceRecorder
    {
        public static void Record(string path, GameModule module)
        {
            Directory.CreateDirectory(path);
            foreach (var unitType in module.UnitTypes.Values)
            {
                var file = $"{path}/{unitType.Key}.txt";
                File.Delete(file);
                using var stream = File.OpenWrite(file);
                using var writer = new StreamWriter(stream);
                Record(unitType, module, writer);
            }
        }

        private static void Record(UnitType unitType, GameModule module, StreamWriter stream)
        {
            foreach (var defender in module.UnitTypes.Values)
            {
                Record(unitType, defender, stream);
            }
        }

        private static void Record(UnitType unitType, UnitType defender, StreamWriter stream)
        {
            stream.WriteLine(WriteUnitType(defender));
            foreach (var weapon in unitType.Weapons)
            {
                foreach (var mode in weapon.Weapon.Modes)
                {
                    Record(unitType, weapon, mode, defender, stream);
                }
            }
        }

        private static void Record(
            UnitType unitType, UnitWeaponUsage weapon, UnitWeapon.Mode mode, UnitType defender, StreamWriter stream)
        {
            stream.WriteLine(WriteMode(weapon.Weapon, mode));
            stream.WriteLine(WriteHeader());
            if (mode.IsIndirect())
            {
                var preview =
                    CombatCalculator.GetIndirectPreview(
                        unitType,
                        mode,
                        defender,
                        CombatCondition.None,
                        unitType.Intrinsics.Number.GetValue() * weapon.Number);
                stream.WriteLine(WritePreview(0, preview));
            }
            else
            {
                for (int range = (int)mode.Range.GetMinimum(); range <= (int)mode.Range.GetMaximum(); ++range)
                {
                    var preview =
                        CombatCalculator.GetDirectPreview(
                            unitType,
                            mode,
                            defender,
                            CombatCondition.None,
                            range,
                            unitType.Intrinsics.Number.GetValue() * weapon.Number);
                    stream.WriteLine(WritePreview(range, preview));
                }
            }
            stream.WriteLine();
        }

        private static string WriteHeader()
        {
            return "Range\tVolume\tSaturation\tTarget\tHit\tPenetrate\tKill\tDiffusion\tResult";
        }

        private static string WriteMode(UnitWeapon weapon, UnitWeapon.Mode mode)
        {
            return $"{weapon.Name}({mode.Condition})";
        }

        private static string WritePreview(int range, CombatPreview preview)
        {
            return $"{range}"
                + $"\t{preview.Volume:N2}"
                + $"\t{preview.Saturation:N2} "
                + $"\t\t{preview.Target.Probability:N2}"
                + $"\t{preview.Hit.Probability:N2}"
                + $"\t{preview.Penetrate.Probability:N2}"
                + $"\t\t{preview.Kill.Probability:N2}"
                + $"\t{preview.Diffusion:N2}"
                + $"\t\t{preview.Result:N2}";
        }

        private static string WriteUnitType(UnitType unitType)
        {
            return unitType.Key;
        }
    }
}

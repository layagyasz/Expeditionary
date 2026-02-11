using Expeditionary.Model.Units;

namespace Expeditionary.Controller.Scenes.Matches
{
    public interface IOrderPrototype
    {
        string Name { get; }

        public record class AttackOrderPrototype(string Name, UnitWeaponUsage Weapon, UnitWeapon.Mode Mode) 
            : IOrderPrototype;

        public record class LoadOrderPrototype(string Name, IAsset Passenger) : IOrderPrototype;

        public record class MoveOrderPrototype(string Name) : IOrderPrototype;

        public record class UnloadOrderPrototype(string Name) : IOrderPrototype;
    }
}

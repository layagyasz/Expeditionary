using Cardamom.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public record class UnitWeaponDistribution
    {
        public bool IsDistributed { get; set; }
        public int Number { get; set; }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public UnitWeapon? Weapon { get; set; }
    }
}

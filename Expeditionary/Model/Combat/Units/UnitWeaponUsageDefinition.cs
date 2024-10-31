using Cardamom.Json;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public class UnitWeaponUsageDefinition
    {
        public bool IsDistributed { get; set; }
        public int Number { get; set; }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public UnitWeaponDefinition? Weapon { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> ExtraTraits { get; set; } = new();

        public UnitWeaponUsage Build()
        {
            return new(IsDistributed, Number, Weapon!.Build(ExtraTraits));
        }
    }
}

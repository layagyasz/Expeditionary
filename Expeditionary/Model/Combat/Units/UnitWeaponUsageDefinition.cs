using Cardamom.Json;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public class UnitWeaponUsageDefinition
    {
        public int Number { get; set; }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public UnitWeaponDefinition? Weapon { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> ExtraTraits { get; set; } = new();

        public UnitWeaponUsage Build()
        {
            return new(Number, Weapon!.Build(ExtraTraits));
        }
    }
}

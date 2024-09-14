using Cardamom.Collections;
using Cardamom.Json.Collections;
using Expeditionary.Model.Combat;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    public class GameModule
    {
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitTrait> UnitTraits { get; set; } = new();
    }
}

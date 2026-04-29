using Cardamom.Collections;
using Cardamom.Json.Collections;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Model.Factions;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.Generator
{
    public record class MissionNode
    {
        public required IEnvironmentProvider Environment { get; set; }
        public required EnumSet<MissionDifficulty> Difficulty { get; set; }

        public required EnumSet<MissionScale> Scale { get; set; }
        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public required List<Faction> Attackers { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public required List<Faction> Defenders { get; set; }

        public required float Frequency { get; set; }
        public required int Cap { get; set; }
        public required ISampler Duration { get; set; }
        public required IMissionContentGenerator Content { get; set; }

        public IEnumerable<Faction> Factions => Enumerable.Concat(Attackers, Defenders);
    }
}

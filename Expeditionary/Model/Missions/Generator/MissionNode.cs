using Cardamom.Collections;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Model.Factions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.Generator
{
    public record class MissionNode
    {
        public required IEnvironmentProvider Environment { get; set; }
        public required EnumSet<MissionDifficulty> Difficulty { get; set; }
        public required EnumSet<MissionScale> Scale { get; set; }
        public required List<Faction> Attackers { get; set; }
        public required List<Faction> Defenders { get; set; }
        public required float Frequency { get; set; }
        public required ISampler Duration { get; set; }
        public required IMissionContentGenerator Content { get; set; }
        
        public Mission Create(MissionGenerationResources resources)
        {
            var content = Content.Generate(this, resources);
            var sector = resources.Galaxy.Sectors[content.Map.Environment.Location.Sector];
            var random = new Random(content.Map.Environment.Location.SystemSeed());
            return new(sector.TopLeft + sector.Size * new Vector2(random.NextSingle(), random.NextSingle()), content);
        }
    }
}

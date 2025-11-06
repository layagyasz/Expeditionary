using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Mapping.Generator;

namespace Expeditionary.Model.Mapping.Environments
{
    public record class MapEnvironment
    {
        public required MapEnvironmentKey Location { get; set; }
        public string? Name { get; set; }
        public required MapAppearance Appearance { get; set; }
        public required MapGenerator.Parameters Parameters { get; set; }
    }
}

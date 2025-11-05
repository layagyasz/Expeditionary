using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Mapping.Generator;

namespace Expeditionary.Model.Mapping.Environments
{
    public record class MapEnvironment(MapEnvironmentName Name)
    {
        public MapAppearance Appearance { get; set; } = new();
        public MapGenerator.Parameters Parameters { get; set; } = new();
    }
}

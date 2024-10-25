using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Mapping.Generator;

namespace Expeditionary.Model.Mapping
{
    public record class MapEnvironment
    {
        public MapAppearance Appearance { get; set; } = new();
        public MapGenerator.Parameters Parameters { get; set; } = new();
    }
}

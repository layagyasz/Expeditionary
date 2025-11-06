using OpenTK.Mathematics;

namespace Expeditionary.Model.Galaxies
{
    public record class Sector
    {
        public required int Id { get; set; }
        public required Vector2 TopLeft { get; set; }
        public required Vector2 Size { get; set; }
        public required int SystemCount { get; set; }
    }
}

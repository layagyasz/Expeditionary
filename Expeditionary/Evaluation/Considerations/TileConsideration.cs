using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.Considerations
{
    public delegate float TileConsideration(Vector3i hex, Tile tile);
}

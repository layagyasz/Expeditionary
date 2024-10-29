namespace Expeditionary.Model.Knowledge
{
    public record struct TileKnowledge(bool IsDiscovered, int VisibilityCounter)
    {
        public bool IsVisible => VisibilityCounter > 0;
    }
}

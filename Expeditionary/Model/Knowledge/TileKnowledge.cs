namespace Expeditionary.Model.Knowledge
{
    public record class TileKnowledge(bool IsDiscovered, int VisibilityCounter)
    {
        public bool IsVisible { get => VisibilityCounter > 0; }
    }
}

namespace Expeditionary.Model.Mapping
{
    public record struct Structure
    {
        public StructureType Type { get; set; }
        public int Level { get; set; }
    }
}

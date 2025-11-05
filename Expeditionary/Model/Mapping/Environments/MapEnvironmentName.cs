namespace Expeditionary.Model.Mapping.Environments
{
    public class MapEnvironmentName
    {
        public string? FixedName { get; set; }
        public MapEnvironmentKey? KeyedName { get; set; }

        public MapEnvironmentName() { }

        public MapEnvironmentName(string name)
        {
            FixedName = name;
        }

        public MapEnvironmentName(MapEnvironmentKey key)
        {
            KeyedName = key;
        }
    }
}

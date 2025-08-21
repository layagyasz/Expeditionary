using Cardamom;
using NLua;

namespace Expeditionary.Model.Mapping.Environments
{
    public class MapEnvironmentTrait : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public LuaFunction? Apply { get; set; }
    }
}

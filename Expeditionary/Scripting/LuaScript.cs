using NLua;

namespace Expeditionary.Scripting
{
    public class LuaScript : IDisposable
    {
        private Lua? _lua;

        private LuaScript(Lua lua)
        {
            _lua = lua;
        }

        public static LuaScript FromFile(string path)
        {
            var lua = new Lua();
            lua.DoFile(path);
            return new(lua);
        }

        public LuaFunction Get(string name)
        {
            return _lua!.GetFunction(name);
        }

        public void Dispose()
        {
            _lua!.Dispose();
            _lua = null;
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }
    }
}

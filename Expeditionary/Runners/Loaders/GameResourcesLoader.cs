using Cardamom;

namespace Expeditionary.Runners.Loaders
{
    public class GameResourcesLoader
    {
        private readonly string _path;

        public GameResourcesLoader(string path)
        {
            _path = path;
        }

        public GameResources Load()
        {
            return GameResources.Builder.ReadFrom(_path).Build();
        }
    }
}

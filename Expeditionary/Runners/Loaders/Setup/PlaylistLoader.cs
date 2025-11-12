using Cardamom.Audio;
using System.Text.Json;

namespace Expeditionary.Runners.Loaders
{
    public class PlaylistLoader
    {
        private readonly string _path;

        public PlaylistLoader(string path)
        {
            _path = path;
        }

        public Playlist Load()
        {
            return JsonSerializer.Deserialize<Playlist>(File.ReadAllText(_path))!;
        }
    }
}

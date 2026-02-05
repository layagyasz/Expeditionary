using System.Text.Json;

namespace Expeditionary.View
{
    public class Localization
    {
        private readonly string _path;

        private string _language = string.Empty;
        private Dictionary<string, string> _dict = new();

        public Localization(string path)
        {
            _path = path;
        }

        public string GetValueOrDefault(string key, string defaultValue)
        {
            return _dict.GetValueOrDefault(key, defaultValue);
        }

        public void SetLanguage(string language)
        {
            _language = language;
            _dict = Load(language);
        }

        public bool TryGetValue(string key, out string? value)
        {
            return _dict.TryGetValue(key, out value);
        }

        private Dictionary<string, string> Load(string language)
        {
            string dir = _path + "/" + language;
            var result = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(dir))
            {
                var contents = JsonSerializer.Deserialize<Dictionary<string, string>>(file)!;
                foreach (var entry in contents)
                {
                    result.Add(entry.Key, entry.Value);
                }
            }
            return result;
        }
    }
}

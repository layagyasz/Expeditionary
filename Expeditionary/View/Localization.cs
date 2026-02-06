using System.Text.Json;

namespace Expeditionary.View
{
    public class Localization
    {
        public EventHandler<EventArgs>? LanguageUpdated { get; set; }

        private readonly string _path;

        private string _language = string.Empty;
        private Dictionary<string, string> _dict = new();

        public Localization(string path)
        {
            _path = path;
        }

        public string Localize(string key, params object?[] args)
        {
            if (TryGetValue(key, out var value))
            {
                return string.Format(value!, args);
            }
            return "Missing Localization";
        }

        public void SetLanguage(string language)
        {
            _language = language;
            _dict = Load(language);
            LanguageUpdated?.Invoke(this, EventArgs.Empty);
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
                using (FileStream stream = new(file, FileMode.Open))
                {
                    var contents = JsonSerializer.Deserialize<Dictionary<string, string>>(stream)!;
                    foreach (var entry in contents)
                    {
                        result.Add(entry.Key, entry.Value);
                    }
                }
            }
            return result;
        }
    }
}

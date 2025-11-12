using Expeditionary.Spectra;
using System.Text.Json;

namespace Expeditionary.Runners.Loaders
{
    public class SpectrumSensitivityLoader
    {
        private readonly string _path;

        public SpectrumSensitivityLoader(string path)
        {
            _path = path;
        }

        public SpectrumSensitivity Load()
        {
            return JsonSerializer.Deserialize<SpectrumSensitivity>(File.ReadAllText(_path))!;
        }
    }
}

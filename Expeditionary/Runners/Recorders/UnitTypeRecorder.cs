using Expeditionary.Model;
using Expeditionary.Model.Units;
using System.Text.Json;

namespace Expeditionary.Runners.Recorders
{
    public static class UnitTypeRecorder
    {
        public static void Record(string path, GameModule module)
        {
            Directory.CreateDirectory(path);
            foreach (var unitType in module.UnitTypes.Values)
            {
                var file = $"{path}/{unitType.Key}.json";
                File.Delete(file);
                using var stream = File.OpenWrite(file);
                using var writer = new StreamWriter(stream);
                Record(unitType, writer);
            }
        }

        private static void Record(UnitType unitType, StreamWriter stream)
        {
            var options =
                new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
            JsonSerializer.Serialize(stream.BaseStream, unitType, typeof(UnitType), options);
        }
    }
}

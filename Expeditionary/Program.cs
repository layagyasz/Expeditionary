using Expeditionary.Runners;

namespace Expeditionary
{
    public class Program
    {
        public static readonly ProgramConfig Config = 
            new(
                "resources/config/default/module.json", 
                "resources/view/ui.json", 
                "resources/audio/playlist.json",
                "resources/view/human_eye_sensitivity.json",
                "resources/view/unit_texture_generator_settings.json");

        public static void Main()
        {
            new LoadRunner(Config).Run();
        }
    }
}

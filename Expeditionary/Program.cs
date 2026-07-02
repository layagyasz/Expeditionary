using Expeditionary.Runners;
using Expeditionary.Runners.GameStates;

namespace Expeditionary
{
    public class Program
    {
        public static readonly ProgramConfig Config = 
            new(
                "resources/config/default/module.json", 
                "resources/view/ui.json", 
                "resources/audio/playlist.json",
                "resources/view/localization",
                "resources/view/human_eye_sensitivity.json",
                "resources/view/unit_texture_generator_settings.json",
                IsDebug: false);

        public static void Main()
        {
            new DefaultRunner(Config, _ => new IGameStateContext.MainMenuContext()).Run();
        }
    }
}

using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Reporting;
using Expeditionary.Runners;
using Expeditionary.Runners.GameStates;
using System.Collections.Immutable;

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
            new DefaultRunner(
                Config, 
                module => new IGameStateContext.MatchSummaryContext(
                    null, 
                    null,
                    new MatchPlayer(0, 0, module.Factions.Values.First()), 
                    new MatchReport(ImmutableDictionary.Create<MatchPlayer, PlayerReport>())))
                .Run();
        }
    }
}

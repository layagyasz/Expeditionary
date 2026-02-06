using Expeditionary.View;

namespace Expeditionary.Runners.Loaders
{
    public class ProgramDataLoader
    {
        private readonly ProgramConfig _config;

        public ProgramDataLoader(ProgramConfig config)
        {
            _config = config;
        }

        public ProgramData Load()
        {
            var resources = new GameResourcesLoader(_config.Resources).Load();
            var module = new ModuleLoader(_config.Module).Load();
            var localization = new Localization(_config.Localization);
            localization.SetLanguage("en");
            return new(
                _config,
                resources, 
                module, 
                new PlaylistLoader(_config.Playlist).Load(),
                localization,
                new SpectrumSensitivityLoader(_config.SpectrumSensitivity).Load(), 
                new RuntimeTextureLibraryLoader(resources).Load(),
                new RuntimeUnitTextureLoader(module, _config.UnitTextureSettings).Load());
        }
    }
}

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
            return new(
                _config,
                resources, 
                module, 
                new PlaylistLoader(_config.Playlist).Load(),
                new SpectrumSensitivityLoader(_config.SpectrumSensitivity).Load(), 
                new RuntimeTextureLibraryLoader(resources).Load(),
                new RuntimeUnitTextureLoader(module, _config.UnitTextureSettings).Load());
        }
    }
}

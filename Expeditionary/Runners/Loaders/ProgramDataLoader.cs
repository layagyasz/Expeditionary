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
                resources, 
                module, 
                new SpectrumSensitivityLoader(_config.SpectrumSensitivity).Load(), 
                new RuntimeTextureLibraryLoader(resources).Load(),
                new RuntimeUnitTextureLoader(module, _config.UnitTextureSettings).Load());
        }
    }
}

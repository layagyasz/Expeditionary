using Expeditionary.Runners.Loaders;
using Expeditionary.Runners.Recorders;

namespace Expeditionary.Runners
{
    public class BalanceRunner : IProgramRunner
    {
        private readonly ProgramConfig _config;

        public BalanceRunner(ProgramConfig config)
        {
            _config = config;
        }

        public void Run() 
        {
            var module = new ModuleLoader(_config.Module).Load();
            UnitTypeRecorder.Record("units", module);
            BalanceRecorder.Record("balance", module);
        }
    }
}

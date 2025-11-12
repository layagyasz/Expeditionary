namespace Expeditionary.Runners
{
    public class DefaultRunner : UiRunner
    {
        public DefaultRunner(ProgramConfig config) 
            : base(config) { }

        protected override void Handle(ProgramController controller) { }
    }
}

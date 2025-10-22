namespace Expeditionary.Loader
{
    public class SourceLoaderTask<T> : LoaderTaskNode<T>
    {
        private readonly Func<T> _func;

        public SourceLoaderTask(Func<T> func, bool isGL)
            : base(isGL)
        {
            _func = func;
        }

        public override IEnumerable<ILoaderTask> GetParents()
        {
            yield break;
        }

        public override bool IsReady()
        {
            return true;
        }

        public override void Notify(ILoaderTask parent) { }

        public override void Perform()
        {
            _promise.Set(_func.Invoke());
        }
    }
}

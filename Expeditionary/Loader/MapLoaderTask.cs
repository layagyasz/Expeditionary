namespace Expeditionary.Loader
{
    public class MapLoaderTask<TIn, TOut> : LoaderTaskNode<TOut>
    {
        private readonly LoaderTaskNode<TIn> _parent;
        private readonly Func<TIn, TOut> _map;

        public MapLoaderTask(LoaderTaskNode<TIn> parent, Func<TIn, TOut> map, bool isGL)
            : base(isGL)
        {
            _parent = parent;
            _map = map;
        }

        public override IEnumerable<ILoaderTask> GetParents()
        {
            yield return _parent;
        }

        public override bool IsReady()
        {
            return _parent.IsDone();
        }

        public override void Notify(ILoaderTask parent) { }

        public override void Perform()
        {
            _promise.Set(_map(_parent.GetPromise().Get()));
        }
    }
}

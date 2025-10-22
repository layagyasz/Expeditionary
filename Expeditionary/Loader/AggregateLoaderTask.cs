namespace Expeditionary.Loader
{
    public class AggregateLoaderTask<TIn, TOut> : LoaderTaskNode<TOut>
    {
        private readonly List<LoaderTaskNode<TIn>> _parents;
        private readonly Func<TOut> _seed;
        private readonly Func<TOut, TIn, TOut> _accumulate;

        private readonly HashSet<ILoaderTask> _outstanding;

        private AggregateLoaderTask(
            IEnumerable<LoaderTaskNode<TIn>> parents, Func<TOut> seed, Func<TOut, TIn, TOut> accumulate, bool isGL)
            : base(isGL)
        {
            _parents = parents.ToList();
            _seed = seed;
            _accumulate = accumulate;

            _outstanding = new(parents);
        }

        public static AggregateLoaderTask<TIn, TOut> Aggregate(
            IEnumerable<LoaderTaskNode<TIn>> parents, Func<TOut> seed, Func<TOut, TIn, TOut> accumulate)
        {
            var child = new AggregateLoaderTask<TIn, TOut>(parents, seed, accumulate, /* isGL= */ false);
            foreach (var parent in parents)
            {
                parent.AddChild(child);
            }
            return child;
        }

        public override IEnumerable<ILoaderTask> GetParents()
        {
            return _parents;
        }

        public override bool IsReady()
        {
            return _outstanding.Count == 0;
        }

        public override void Notify(ILoaderTask parent)
        {
            _outstanding.Remove(parent);
        }

        public override void Perform()
        {
            _promise.Set(_parents.Select(x => x.GetPromise().Get()).Aggregate(_seed(), _accumulate));
        }
    }
}

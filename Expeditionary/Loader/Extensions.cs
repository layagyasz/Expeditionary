namespace Expeditionary.Loader
{
    public static class Extensions
    {
        public static MapLoaderTask<TIn, TOut> Map<TIn, TOut>(this LoaderTaskNode<TIn> node, Func<TIn, TOut> map)
        {
            var child = new MapLoaderTask<TIn, TOut>(node, map, /* isGL= */ false);
            node.AddChild(child);
            return child;
        }

        public static MapLoaderTask<TIn, TOut> MapGL<TIn, TOut>(this LoaderTaskNode<TIn> node, Func<TIn, TOut> map)
        {
            var child = new MapLoaderTask<TIn, TOut>(node, map, /* isGL= */ true);
            node.AddChild(child);
            return child;
        }
    }
}

namespace Expeditionary.Loader
{
    public interface ILoaderTask
    {
        bool IsGL { get; }
        void DoNow();
        bool IsDone();
        bool IsReady();
        IEnumerable<ILoaderTask> GetParents();
        IEnumerable<ILoaderTask> GetChildren();
        void Notify(ILoaderTask parent);
        void Perform();
    }
}

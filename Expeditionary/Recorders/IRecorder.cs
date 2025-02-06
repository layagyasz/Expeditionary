namespace Expeditionary.Recorders
{
    public interface IRecorder
    {
        void Record(StreamWriter stream, RecorderContext context, object? @object, int depth = 0);
    }
}

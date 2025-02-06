namespace Expeditionary.Recorders
{
    public class StringRecorder : IRecorder
    {
        public void Record(StreamWriter stream, RecorderContext context, object? @object, int depth = 0)
        {
            stream.WriteLine(@object?.ToString() ?? "null");
        }
    }
}

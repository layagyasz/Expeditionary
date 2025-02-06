namespace Expeditionary.Recorders
{
    public class DefaultRecorder : IRecorder
    {
        public void Record(StreamWriter stream, RecorderContext context, object? @object, int depth = 0)
        {
            if (@object == null)
            {
                stream.WriteLine("null");
                return;
            }
            var type = @object.GetType();
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic)
                {
                    continue;
                }
                Recorder.Record(stream, context, @object, field, depth);
            }
            foreach (var property in type.GetProperties())
            {
                Recorder.Record(stream, context, @object, property, depth);
            }
        }
    }
}

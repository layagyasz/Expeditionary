using System.Reflection;

namespace Expeditionary.Recorders
{
    public static class Recorder
    {
        public static void Record(
            StreamWriter stream, RecorderContext context, object? @object, FieldInfo field, int depth = 0)
        {
            context.GetRecorder(field)
                .Record(
                    stream,
                    context,
                    field.GetValue(@object),
                    WritePreamble(stream, field.FieldType, field.Name, depth));
        }

        public static void Record(
            StreamWriter stream, RecorderContext context, object? @object, PropertyInfo property, int depth = 0)
        {
            context.GetRecorder(property)
                .Record(
                    stream, 
                    context,
                    property.GetValue(@object),
                    WritePreamble(stream, property.PropertyType, property.Name, depth));
        }

        public static void Record(StreamWriter stream, RecorderContext context, object? @object, int depth = 0)
        {
            if (@object == null)
            {
                stream.WriteLine("null");
                return;
            }
            context.GetRecorder(@object.GetType()).Record(stream, context, @object, depth);
            stream.WriteLine();
        }

        private static int WritePreamble(StreamWriter stream, Type type, string name, int depth)
        {
            string padding = "".PadLeft(depth, '\t');
            if (type.IsPrimitive || type == typeof(string))
            {
                stream.Write($"{padding}{name} = ");
                return depth;
            }
            else
            {
                stream.WriteLine($"{padding}[{name}]");
                return depth + 1;
            }
        }
    }
}

using System.Reflection;

namespace Expeditionary.Recorders
{
    public class RecorderContext
    {
        private readonly Dictionary<Type, IRecorder> _recorders = new();

        public IRecorder GetRecorder(FieldInfo field)
        {
            return GetRecorderOfType(
                GetRecorderType(field.GetCustomAttributes(false)) ?? GetRecorderTypeFor(field.FieldType));
        }

        public IRecorder GetRecorder(PropertyInfo property)
        {
            return GetRecorderOfType(
                GetRecorderType(property.GetCustomAttributes(false)) ?? GetRecorderTypeFor(property.PropertyType));
        }

        public IRecorder GetRecorder(Type type)
        {
            return GetRecorderOfType(GetRecorderTypeFor(type));
        }

        private IRecorder GetRecorderOfType(Type type)
        {
            if (_recorders.TryGetValue(type, out var recorder))
            {
                return recorder;
            }
            var newRecorder = (IRecorder)Activator.CreateInstance(type)!;
            _recorders.Add(type, newRecorder);
            return newRecorder;
        }

        private static Type GetRecorderTypeFor(Type type)
        {
            return GetRecorderType(type.GetCustomAttributes(false)) ?? GetDefaultRecorder(type);
        }

        private static Type? GetRecorderType(IEnumerable<object> attributes)
        {
            return ((RecorderAttribute?)attributes.FirstOrDefault(x => x is RecorderAttribute))?.Type;
        }

        private static Type GetDefaultRecorder(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
            {
                return typeof(StringRecorder);
            }
            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType 
                    && @interface.GetGenericTypeDefinition().IsAssignableTo(typeof(IEnumerable<>)))
                {
                    return typeof(CollectionRecorder);
                }
            }
            return typeof(DefaultRecorder);
        }
    }
}

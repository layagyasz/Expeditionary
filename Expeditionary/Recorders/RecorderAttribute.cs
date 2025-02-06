namespace Expeditionary.Recorders
{
    [AttributeUsage(
        AttributeTargets.Field 
        | AttributeTargets.Property
        | AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Enum)]
    public class RecorderAttribute : Attribute
    {
        public Type Type { get; set; }

        public RecorderAttribute(Type type)
        {
            Type = type;
        }
    }
}

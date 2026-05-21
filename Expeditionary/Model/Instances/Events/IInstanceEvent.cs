namespace Expeditionary.Model.Instances.Events
{
    public interface IInstanceEvent
    {
        bool Apply(GameInstance instance);
    }
}

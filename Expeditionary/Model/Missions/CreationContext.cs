using Expeditionary.Loader;

namespace Expeditionary.Model.Missions
{
    public record class CreationContext(LoaderStatus Status, Player Player, bool IsTest);
}

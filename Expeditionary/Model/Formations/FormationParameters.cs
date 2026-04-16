using Cardamom.Collections;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    // TODO: Implement point limits
    public record class FormationParameters(
        Faction Faction,
        EnumSet<FormationRole> AllowedRoles,
        EnumSet<UnitTag> RequiredTags,
        EnumSet<UnitTag> ExcludedTags,
        Random Random);
}

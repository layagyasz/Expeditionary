using Cardamom.Collections;
using Expeditionary.Model.Factions;
using System.Collections.Immutable;

namespace Expeditionary.Model.Formations.Generator
{
    // TODO: Implement point limits
    public record class FormationParameters(
        float Points,
        int Echelon,
        Faction Faction,
        EnumSet<FormationRole> AllowedRoles,
        ImmutableList<UnitConstraint> Constraints,
        Random Random);
}

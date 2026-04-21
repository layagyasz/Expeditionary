using System.Collections.Immutable;

namespace Expeditionary.Model.Formations.Generator
{
    public record class FormationGeneratorContext(
        FormationParameters Parameters, ImmutableList<UnitUsage> AvailableUnits);
}

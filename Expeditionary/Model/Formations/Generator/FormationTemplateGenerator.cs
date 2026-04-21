using Cardamom;
using Cardamom.Json;
using Expeditionary.Model.Units;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations.Generator
{
    public record class FormationTemplateGenerator : IKeyed
    {
        public record class ParameterizedFormationGenerator
        {
            public int Number { get; set; }
            public ImmutableList<UnitConstraint> Constraints { get; set; } = ImmutableList.Create<UnitConstraint>();

            [JsonConverter(typeof(ReferenceJsonConverter))]
            public FormationTemplateGenerator? Formation { get; set; }

            public TemplateFormation Generate(FormationGeneratorContext context)
            {
                return Formation!.Generate(
                    context with 
                    { 
                        Parameters = context.Parameters with 
                        { 
                            Constraints = ImmutableList.CreateRange(
                                Enumerable.Concat(context.Parameters.Constraints, Constraints))
                        } 
                    });
            }
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FormationRole Role { get; set; }
        public int Echelon { get; set; }
        public List<ParameterizedFormationGenerator> ComponentFormations { get; set; } = new();
        public List<DiadParameters> Diads { get; set; } = new();

        public TemplateFormation Generate(FormationGeneratorContext context)
        {
            return new(
                Name,
                Role,
                Echelon,
                ComponentFormations.SelectMany(x => Enumerable.Repeat(x.Generate(context), x.Number)).ToList(),
                Diads.SelectMany(x => Enumerable.Repeat(Select(x, context), x.Number)).ToList());
        }

        private static TemplateDiad Select(DiadParameters diad, FormationGeneratorContext context)
        {
            return new(
                diad.Unit.Role,
                Select(context, diad.Unit),
                diad.Transport == null ? null : Select(context, diad.Transport));
        }

        private static UnitType Select(FormationGeneratorContext context, UnitSlot slot)
        {
            var matchingUnits = 
                context.AvailableUnits
                    .Where(slot.Matches)
                    .Where(
                        unit => 
                            context.Parameters.Constraints.All(
                                constraint => constraint.Satisfies(slot, unit.Type!.Tags)))
                    .ToList();
            if (!matchingUnits.Any())
            {
                throw new ArgumentException(
                    $"No matching units for {slot} with constraints {context.Parameters.Constraints}");
            }
            return matchingUnits[context.Parameters.Random.Next(matchingUnits.Count)].Type!;
        }
    }
}

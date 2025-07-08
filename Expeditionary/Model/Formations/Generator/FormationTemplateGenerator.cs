using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations.Generator
{
    public record class FormationTemplateGenerator : IKeyed
    {
        public record class ParameterizedFormationGenerator
        {
            public int Number { get; set; }
            public EnumSet<UnitTag> RequiredTags { get; set; } = new();
            public EnumSet<UnitTag> ExcludedTags { get; set; } = new();

            [JsonConverter(typeof(ReferenceJsonConverter))]
            public FormationTemplateGenerator? Formation { get; set; }

            public FormationTemplate Generate(FormationGeneratorContext context)
            {
                return Formation!.Generate(context.WithTags(RequiredTags, ExcludedTags));
            }
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FormationRole Role { get; set; }
        public int Echelon { get; set; }
        public List<ParameterizedFormationGenerator> ComponentFormations { get; set; } = new();
        public List<DiadTemplate> Diads { get; set; } = new();

        public FormationTemplate Generate(FormationGeneratorContext context)
        {
            return new(
                Name,
                Role,
                Echelon,
                ComponentFormations.SelectMany(x => Enumerable.Repeat(x.Generate(context), x.Number)).ToList(),
                Diads.SelectMany(x => Enumerable.Repeat(Select(x, context), x.Number)).ToList());
        }

        private static FormationTemplate.Diad Select(DiadTemplate diad, FormationGeneratorContext context)
        {
            return new(
                diad.Unit.Role,
                context.Select(diad.Unit)!, 
                diad.Transport == null ? null : context.Select(diad.Transport));
        }
    }
}

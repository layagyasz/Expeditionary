using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations.Generator
{
    public record class FormationGenerator : IKeyed
    {
        public record class ParameterizedFormationGenerator
        {
            public int Number { get; set; }
            public EnumSet<UnitTag> RequiredTags { get; set; } = new();
            public EnumSet<UnitTag> ExcludedTags { get; set; } = new();

            [JsonConverter(typeof(ReferenceJsonConverter))]
            public FormationGenerator? Formation { get; set; }

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
        public List<FormationSlot> UnitSlots { get; set; } = new();

        public FormationTemplate Generate(FormationGeneratorContext context)
        {
            return new(
                Name,
                Role,
                Echelon,
                ComponentFormations.SelectMany(x => Enumerable.Repeat(x.Generate(context), x.Number)).ToList(), 
                UnitSlots.SelectMany(
                    x => Enumerable.Repeat(
                        new FormationTemplate.UnitTypeAndRole(context.Select(x), x.Role), x.Number)).ToList());
        }
    }
}

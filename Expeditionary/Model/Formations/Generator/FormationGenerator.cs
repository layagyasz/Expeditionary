using Cardamom.Collections;
using Expeditionary.Model.Factions;
using System.Collections.Immutable;

namespace Expeditionary.Model.Formations.Generator
{
    public class FormationGenerator
    {
        public Library<FactionFormationConfiguration> FactionFormations { get; }
        public Library<FormationTemplateGenerator> Formations { get; }

        public FormationGenerator(
            Library<FactionFormationConfiguration> factionFormations, Library<FormationTemplateGenerator> formations)
        {
            FactionFormations = factionFormations;
            Formations = formations;
        }

        public TemplateFormation Generate(FormationParameters parameters)
        {
            var formationConfig = 
                FactionFormations.Values.Where(config => config.Faction == parameters.Faction.Key).First();
            var availableGenerators =
                formationConfig.Formations
                    .Where(generator => generator.Echelon == parameters.Echelon)
                    .Where(generator => parameters.AllowedRoles.Contains(generator.Role))
                    .ToList();
            var formationGenerator = availableGenerators[parameters.Random.Next(availableGenerators.Count)];
            return formationGenerator.Generate(new(parameters, formationConfig.Units.ToImmutableList()));
        }
    }
}

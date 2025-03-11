using Cardamom.Collections;
using Expeditionary.Model.Factions;

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

        public FormationTemplate Generate(Faction faction, Random random)
        {
            var formationConfig = FactionFormations.Where(x => x.Value.Faction == faction.Key).First().Value;
            var formationGenerator = formationConfig.Formations[random.Next(formationConfig.Formations.Count)];
            return formationGenerator.Generate(new(random, new(), new(), formationConfig.Units));
        }
    }
}

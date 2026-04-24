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
            var formationConfig = FactionFormations.Values.First(config => config.Faction == parameters.Faction.Key)!;
            var context = new FormationGeneratorContext(parameters, formationConfig.Units.ToImmutableList());
            var generators = EnumerateGenerators(formationConfig.Formations);
            var root = 
                GenerateFromTemplate(generators, context)
                // TODO: Figure out better defaults
                ?? new TemplateFormation(
                    string.Empty,
                    parameters.AllowedRoles.First(),
                    parameters.Echelon, 
                    Enumerable.Empty<TemplateFormation>(),
                    Enumerable.Empty<TemplateDiad>());
            var points = parameters.Points - (root == null ? 0 : root.Value.Points);
            if (points > 0)
            {
                var children =
                    GenerateFill(
                        generators,
                        context with
                        {
                            Parameters = context.Parameters with
                            {
                                Points = points,
                                Echelon = parameters.Echelon - 1
                            }
                        })
                    .ToList();
                root =
                    new TemplateFormation(
                        root!.Name, 
                        root.Role, 
                        root.Echelon,
                        Enumerable.Concat(root.ComponentFormations, children),
                        Enumerable.Empty<TemplateDiad>());
            }
            return Prune(root!, parameters.Points);
        }

        private static IEnumerable<FormationTemplateGenerator> EnumerateGenerators(
            IEnumerable<FormationTemplateGenerator> generators)
        {
            foreach (var generator in generators)
            {
                yield return generator;
                foreach (var child in EnumerateGenerators(
                    generator.ComponentFormations.Select(component => component.Formation!)))
                {
                    yield return child;
                }
            }
        }

        private static TemplateFormation? GenerateFromTemplate(
            IEnumerable<FormationTemplateGenerator> generators, FormationGeneratorContext context)
        {
            var parameters = context.Parameters;
            var availableGenerators = GetGenerators(generators, parameters).ToList();
            if (!availableGenerators.Any())
            {
                return null;
            }
            var formationGenerator = availableGenerators[parameters.Random.Next(availableGenerators.Count)];
            return formationGenerator.Generate(context);
        }

        private static IEnumerable<TemplateFormation> GenerateFill(
            IEnumerable<FormationTemplateGenerator> generators, FormationGeneratorContext context)
        {
            var parameters = context.Parameters;
            var availableGenerators = GetGenerators(generators, parameters).ToList();
            var echelon = parameters.Echelon - 1;
            if (!availableGenerators.Any())
            {
                if (parameters.Echelon == 1)
                {
                    throw new ArgumentException($"No available formation generators for {context}");
                }
                return GenerateFill(
                    generators,
                    context with
                    {
                        Parameters = context.Parameters with
                        {
                            Echelon = echelon
                        }
                    });
            }
            float points = parameters.Points;
            var formations = new List<TemplateFormation>();
            while (points > 0)
            {
                var formationGenerator = availableGenerators[parameters.Random.Next(availableGenerators.Count)];
                var formation =
                    formationGenerator.Generate(
                        context with
                        {
                            Parameters = context.Parameters with
                            {
                                Points = points,
                                Echelon = echelon
                            }
                        });
                formations.Add(formation);
                points -= formation.Value.Points;
            }
            return formations;
        }

        private static IEnumerable<FormationTemplateGenerator> GetGenerators(
            IEnumerable<FormationTemplateGenerator> generators, FormationParameters parameters)
        {
            return generators
                .Where(generator => generator.Echelon == parameters.Echelon)
                .Where(generator => parameters.AllowedRoles.Contains(generator.Role))
                .ToList();
        }

        private static TemplateFormation Prune(TemplateFormation template, float points)
        {
            var usedPoints = 0;
            var filledRoles = new EnumSet<FormationRole>();

            var coreComponents = new Heap<TemplateFormation, float>();
            var extraComponents = new Heap<TemplateFormation, float>();
            foreach (var component in template.ComponentFormations)
            {
                if (filledRoles.Contains(component.Role))
                {
                    extraComponents.Push(component, -component.Value.Points);
                }
                else
                {
                    coreComponents.Push(component, -component.Value.Points);
                    filledRoles.Add(component.Role);
                    usedPoints += component.Value.Points;
                }
                if (usedPoints > points)
                {
                    break;
                }
            }

            var coreDiads = new Heap<TemplateDiad, float>();
            var extraDiads = new Heap<TemplateDiad, float>();
            foreach (var diad in template.Diads)
            {
                if (filledRoles.Contains(diad.Role))
                {
                    extraDiads.Push(diad, -diad.Value.Points);
                }
                else
                {
                    coreDiads.Push(diad, -diad.Value.Points);
                    filledRoles.Add(diad.Role);
                    usedPoints += diad.Value.Points;
                }
                if (usedPoints > points)
                {
                    break;
                }
            }

            while (usedPoints < points)
            {
                if (extraComponents.Any())
                {
                    var component = extraComponents.Pop();
                    coreComponents.Push(component, -component.Value.Points);
                    usedPoints += component.Value.Points;
                }
                else if (extraDiads.Any())
                {
                    var diad = extraDiads.Pop();
                    coreDiads.Push(diad, -diad.Value.Points);
                    usedPoints += diad.Value.Points;
                }
                else
                {
                    break;
                }
            }

            while (usedPoints > points)
            {
                if (coreDiads.Any())
                {
                    var diad = coreDiads.Pop();
                    usedPoints -= diad.Value.Points;
                }
                else if (coreComponents.Any())
                {
                    var component = coreComponents.Pop();
                    var targetPoints = component.Value.Points + points - usedPoints;
                    var pruned = Prune(component, Math.Max(targetPoints, 0.5f * component.Value.Points));
                    coreComponents.Push(pruned, pruned.Value.Points);
                    usedPoints = usedPoints - component.Value.Points + pruned.Value.Points;
                }
            }

            return new(
                template.Name,
                template.Role, 
                template.Echelon,
                coreComponents.Select(kvp => kvp.Key), 
                coreDiads.Select(kvp => kvp.Key));
        }
    }
}

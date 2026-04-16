using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Instances;

namespace Expeditionary.Model.Missions
{
    public interface IFormationProvider
    {
        InstanceFormation Get(FormationParameters parameters);

        public record PersistentFormationProvider(InstanceFormation Formation) : IFormationProvider
        {
            public InstanceFormation Get(FormationParameters parameters)
            {
                return Formation;
            }
        }

        public record RandomFormationProvider(FormationGenerator Generator) : IFormationProvider
        {
            public InstanceFormation Get(FormationParameters parameters)
            {
                return InstanceFormation.From(
                    Generator.Generate(parameters), new StaticIdGenerator(Constants.NoInstanceId));
            }
        }
    }
}

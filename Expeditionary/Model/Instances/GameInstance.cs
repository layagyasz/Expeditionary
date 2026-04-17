using Expeditionary.Model.Formations;
using Expeditionary.Model.Galaxies;

namespace Expeditionary.Model.Instances
{
    public record class GameInstance(
        IIdGenerator IdGenerator, InstancePlayer Player, Galaxy Galaxy, MissionManager Missions)
    {
        public void AddFormation(TemplateFormation template)
        {
            Player.AddFormation(InstanceFormation.From(template, IdGenerator));
        }
    }
}

using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationTemplate Formation, IFormationAssignment Assignment)
    {
        public void Setup(Player player, Match match, SetupContext context)
        {
            var formation = match.Add(player, Formation);
            var handler =
                context.AiManager.GetHandler(player)
                ?? context.AiManager.CreateHandler(player);
            handler.AddFormation(formation, Assignment);
            handler.Setup();
        }
    }
}

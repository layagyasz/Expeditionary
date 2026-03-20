using Expeditionary.Model.Formations;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai.Assignments;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationTemplate Formation, IAssignment Assignment)
    {
        public void Setup(Player player, Match match, SetupContext context)
        {
            var formation = match.Add(player, Formation);
            var handler =
                context.AiManager.GetHandler(player)
                ?? context.AiManager.CreateHandler(player);
            handler.AddFormation(formation, parent: null);
            handler.SetAssignment(Assignment);
            handler.Setup();
        }
    }
}

using Expeditionary.Model.Formations;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai.Assignments;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationParameters FormationParameters, IAssignment Assignment)
    {
        public void Setup(Player player, Match match, SetupContext context)
        {
            var playerContext = context.GetPlayerContext(player);
            var formation = match.Add(player, playerContext.FormationProvider.Get(FormationParameters));
            var handler = 
                playerContext.IsHuman 
                    ? context.AiManager.CreateHandler(player) 
                    : context.AiManager.GetHandler(player)!;
            handler.AddFormation(formation, parent: null);
            handler.SetAssignment(Assignment);
            handler.Setup();
        }
    }
}

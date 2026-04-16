namespace Expeditionary.Model.Missions
{
    public record class PlayerSetupContext(Player Player, bool IsHuman, IFormationProvider FormationProvider);
}

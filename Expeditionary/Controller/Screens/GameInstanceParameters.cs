using Expeditionary.Model.Factions;

namespace Expeditionary.Controller.Screens
{
    public record class GameInstanceParameters(Faction Faction, int Seed);
}

using Expeditionary.Model.Factions;

namespace Expeditionary.Model
{
    public record class Player(int Id, int Team, Faction Faction);
}

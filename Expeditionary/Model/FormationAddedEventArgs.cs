using Expeditionary.Model.Formations;

namespace Expeditionary.Model
{
    public record class FormationAddedEventArgs(Formation Formation, Formation? Parent);
}

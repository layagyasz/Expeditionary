using Expeditionary.Model.Matches.Ai.Assignments;

namespace Expeditionary.Model.Matches.Ai
{
    public interface IAiHandler
    {
        public IAssignment Assignment { get; }
        public IEnumerable<FormationHandler> Components { get; }
        public IEnumerable<DiadHandler> Diads { get; }
        public string Id { get; }
        public int Echelon { get; }
        public void AddComponent(FormationHandler handler);
        public AiHandlerStatus DoTurn(Match match);
        public Movement.Hindrance GetMaxHindrance();
        public void Setup(Match match);
        public void SetAssignment(IAssignment assignment);
    }
}

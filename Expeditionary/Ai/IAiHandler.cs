using Expeditionary.Ai.Assignments;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public interface IAiHandler
    {
        public IAssignment Assignment { get; }
        public IEnumerable<FormationHandler> Children { get; }
        public IEnumerable<DiadHandler> Diads { get; }
        public string Id { get; }
        public int Echelon { get; }
        public void Add(FormationHandler handler);
        public void DoTurn(Match match);
        public Movement.Hindrance GetMaxHindrance();
        public void Setup(Match match);
        public void SetAssignment(IAssignment assignment);
    }
}

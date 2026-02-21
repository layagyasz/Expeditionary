
using Cardamom.Ui.Controller;
using Expeditionary.View.Common.Components;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes.Matches.Overlays;

namespace Expeditionary.View.Screens
{
    public class MatchScreen : BaseScreen
    {
        public MatchScene Scene { get; private set; }
        public FinishedOverlay FinishedOverlay { get; private set; }
        public ObjectiveOverlay ObjectiveOverlay { get; private set; }
        public UnitOverlay UnitOverlay { get; private set; }
        public RightClickMenu UnitSelect { get; private set; }

        public MatchScreen(
            IController controller,
            MatchScene scene,
            FinishedOverlay finishedOverlay,
            ObjectiveOverlay objectiveOverlay,
            UnitOverlay unitOverlay, 
            RightClickMenu unitSelect)
            : base(controller)
        {
            Scene = scene;
            Register(Scene);

            FinishedOverlay = finishedOverlay;
            Register(FinishedOverlay);

            ObjectiveOverlay = objectiveOverlay;
            Register(ObjectiveOverlay);

            UnitOverlay = unitOverlay;
            Register(UnitOverlay);

            UnitSelect = unitSelect;
            Register(UnitSelect);
        }

    }
}

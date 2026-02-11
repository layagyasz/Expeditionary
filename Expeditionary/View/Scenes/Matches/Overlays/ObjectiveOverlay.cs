using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Model;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.View.Common.Components.Dynamics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Overlays
{
    public class ObjectiveOverlay : DynamicUiCompoundComponent
    {
        private const string Container = "objective-overlay-container";
        private const string Title = "objective-overlay-title";
        private const string ObjectiveContainer = "objective-overlay-objective-container";
        private const string ObjectiveText = "objective-overlay-objective-text";

        private const string TitleKey = "localize-objective-overlay-title";

        public ObjectiveOverlay(IController controller, IUiContainer container) 
            : base(controller, container) { }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Position = new(0, Position.Y, 0);
        }

        public static ObjectiveOverlay Create(
            UiElementFactory uiElementFactory, Localization localization, Match match, Player player)
        {
            return new(
                new NoOpController(), 
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(Container), 
                    new NoOpElementController(), 
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(Title), 
                        new InlayController(uiElementFactory.GetAudioPlayer()),
                        localization.Localize(TitleKey)),
                    CreateElement(uiElementFactory, match, player, match.GetObjective(player))
                });
        }

        private static IUiElement CreateElement(
            UiElementFactory uiElementFactory, Match match, Player player, IObjective objective)
        {
            return new DynamicTextUiElement(
                uiElementFactory.GetClass(ObjectiveText), 
                new InlayController(uiElementFactory.GetAudioPlayer()), 
                () => ToObjectiveString(objective, objective.GetProgress(player, match)));
        }

        private static string ToObjectiveString(IObjective objective, ObjectiveProgress progress)
        {
            return $"{progress.Progress}/{progress.Target} Done";
        }
    }
}

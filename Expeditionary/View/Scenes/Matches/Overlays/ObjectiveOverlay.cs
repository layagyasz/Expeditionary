using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Matches.Overlays;
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

        private const string TitleKey = "localize-objective-overlay-title";

        public class ObjectiveComponent : DynamicUiCompoundComponent
        {
            private const string Container = "objective-overlay-objective-container";
            private const string Check = "objective-overlay-objective-checkbox";
            private const string Text = "objective-overlay-objective-text";

            public IUiElement Checkbox { get; }

            private ObjectiveComponent(IController controller, IUiContainer container, IUiElement checkbox)
                : base(controller, container)
            {
                Checkbox = checkbox;
            }

            public static ObjectiveComponent Create(
                UiElementFactory uiElementFactory, Match match, Player player, IObjective objective)
            {
                var container = 
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(Container),
                        new InlayController(uiElementFactory.GetAudioPlayer()),
                        UiSerialContainer.Orientation.Horizontal);

                var check = 
                    new SimpleUiElement(
                        uiElementFactory.GetClass(Check), 
                        new OptionElementController<object>(uiElementFactory.GetAudioPlayer(), objective));
                container.Add(check);

                var text = 
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(Text),
                        new InlayController(uiElementFactory.GetAudioPlayer()),
                        () => ToObjectiveString(objective, objective.GetProgress(player, match)));
                container.Add(text);

                return new ObjectiveComponent(
                    new ObjectiveComponentController(match, player, objective), container, check);
            }

            private static string ToObjectiveString(IObjective objective, ObjectiveProgress progress)
            {
                return $"{progress.Progress}/{progress.Target} Done";
            }
        }

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
                    ObjectiveComponent.Create(uiElementFactory, match, player, match.GetObjective(player))
                });
        }
    }
}

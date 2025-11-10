using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Common;
using Expeditionary.View.Common;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class MissionPane : UiCompoundComponent, IPane
    {
        private static readonly string s_Container = "galaxy-mission-pane";
        private static readonly string s_Close = "galaxy-mission-pane-close";
        private static readonly string s_Title = "galaxy-mission-pane-title";

        public IUiElement CloseButton { get; }

        private readonly TextUiElement _title;

        private MissionPane(
            IController controller, IUiContainer container, TextUiElement title, IUiElement closeButton)
            : base(controller, container)
        {
            _title = title;
            CloseButton = closeButton;

            Add(_title);
            closeButton.Position = new Vector3(Size.X - _title.RightPadding.X - closeButton.Size.X, 0, 0);
            Add(closeButton);
        }

        public static MissionPane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new SimplePaneController(),
                new UiContainer(
                    uiElementFactory.GetClass(s_Container), new PaneController(uiElementFactory.GetAudioPlayer())), 
                new TextUiElement(
                    uiElementFactory.GetClass(s_Title), 
                    new InlayController(uiElementFactory.GetAudioPlayer()), 
                    string.Empty),
                new SimpleUiElement(
                    uiElementFactory.GetClass(s_Close), 
                    new SimpleElementController(uiElementFactory.GetAudioPlayer())))
            {
                Visible = false
            };
        }

        public void SetTitle(string title)
        {
            _title.SetText(title);
        }
    }
}

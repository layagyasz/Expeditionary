using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.View.Common;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches
{
    public class UnitOverlay : UiCompoundComponent
    {
        private static readonly string s_Container = "unit-overlay-container";
        private static readonly string s_TitleContainer = "unit-overlay-title-container";
        private static readonly string s_Title = "unit-overlay-title";
        private static readonly string s_OrderContainer = "unit-overlay-order-container";
        private static readonly ButtonStyle s_AttackButton = 
            new("base-button", "unit-overlay-order-attack", "base-button-text");
        private static readonly ButtonStyle s_MoveButton =
            new("base-button", "unit-overlay-order-move", "base-button-text");

        public TextUiElement Title { get; }
        public UiCompoundComponent Orders { get; }

        public UnitOverlay(UiElementFactory uiElementFactory)
            : base(
                  new UnitOverlayController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Orders =
                new UiCompoundComponent(
                    new RadioController<ButtonId>(),
                    uiElementFactory.CreateTableRow(
                        s_OrderContainer, 
                        new List<IUiElement>()
                        {
                            uiElementFactory.CreateButton(
                                s_AttackButton, new OptionElementController<ButtonId>(ButtonId.Attack), "Attack"),
                            uiElementFactory.CreateButton(
                                s_MoveButton, new OptionElementController<ButtonId>(ButtonId.Move), "Move")
                        },
                        new InlayController()));
            Title = new TextUiElement(uiElementFactory.GetClass(s_Title), new InlayController(), string.Empty);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_TitleContainer),
                    new InlayController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    Title
                });
            Add(Orders);
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Position = new(bounds.X - Size.X, Position.Y, 0);
        }
    }
}

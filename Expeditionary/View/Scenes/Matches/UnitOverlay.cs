using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Matches;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches
{
    public class UnitOverlay : UiCompoundComponent
    {
        private static readonly string s_Container = "unit-overlay-container";
        private static readonly string s_OrderContainer = "unit-overlay-order-container";
        private static readonly string s_AttackButton = "unit-overlay-order-attack";
        private static readonly string s_MoveButton = "unit-overlay-order-move";

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
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_AttackButton),
                                new OptionElementController<ButtonId>(ButtonId.Attack)),
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_MoveButton),
                                new OptionElementController<ButtonId>(ButtonId.Move))
                        },
                        new InlayController()));
            Add(Orders);
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Position = new(bounds.X - Size.X, Position.Y, 0);
        }
    }
}

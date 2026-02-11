using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Controller.Scenes.Matches.Overlays;
using Expeditionary.View.Common;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Overlays
{
    public class UnitOverlay : UiCompoundComponent
    {
        private static readonly string s_Container = "unit-overlay-container";
        private static readonly string s_TitleContainer = "unit-overlay-title-container";
        private static readonly string s_Title = "unit-overlay-title";
        private static readonly string s_OrderContainer = "unit-overlay-order-container";
        private static readonly ButtonStyle s_AttackButton =
            new("unit-overlay-order-button", "unit-overlay-order-attack", "base-button-text");
        private static readonly ButtonStyle s_MoveButton =
            new("unit-overlay-order-button", "unit-overlay-order-move", "base-button-text");
        private static readonly ButtonStyle s_LoadButton =
            new("unit-overlay-order-button", "unit-overlay-order-load", "base-button-text");
        private static readonly ButtonStyle s_UnloadButton =
            new("unit-overlay-order-button", "unit-overlay-order-unload", "base-button-text");

        public TextUiElement Title { get; }
        public UiCompoundComponent Orders { get; }

        private readonly UiElementFactory _uiElementFactory;

        public UnitOverlay(UiElementFactory uiElementFactory)
            : base(
                  new UnitOverlayController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;

            Orders =
                new UiCompoundComponent(
                    new RadioController<IOrderPrototype>(),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_OrderContainer),
                        new InlayController(uiElementFactory.GetAudioPlayer()),
                        UiSerialContainer.Orientation.Vertical));
            Title =
                new TextUiElement(
                    uiElementFactory.GetClass(s_Title),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    string.Empty);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_TitleContainer),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    Title
                });
            Add(Orders);
        }

        public void AddOrder(IOrderPrototype order)
        {
            var element =
                _uiElementFactory.CreateButton(
                    GetClass(order),
                    new OptionElementController<IOrderPrototype>(_uiElementFactory.GetAudioPlayer(), order),
                    order.Name);
            element.Initialize();
            Orders.Add(element);
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Position = new(bounds.X - Size.X, Position.Y, 0);
        }

        private static ButtonStyle GetClass(IOrderPrototype order)
        {
            return order switch
            {
                IOrderPrototype.AttackOrderPrototype => s_AttackButton,
                IOrderPrototype.MoveOrderPrototype => s_MoveButton,
                IOrderPrototype.LoadOrderPrototype => s_LoadButton,
                IOrderPrototype.UnloadOrderPrototype => s_UnloadButton,
                _ => throw new ArgumentException($"Unsupported IOrderPrototype: {order}"),
            };
        }
    }
}

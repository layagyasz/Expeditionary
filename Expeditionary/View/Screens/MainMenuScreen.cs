using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.View.Common.Components;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class MainMenuScreen : GraphicsResource, IRenderable
    {
        public static readonly object NewGame = new();
        public static readonly object LoadGame = new();
        public static readonly object Options = new();

        private static readonly string s_Container = "main-menu-container";
        private static readonly string s_Option = "main-menu-option";

        public IController Controller { get; }
        public ButtonMenu? Menu { get; private set; }

        public MainMenuScreen(IController controller, ButtonMenu menu)
        {
            Controller = controller;
            Menu = menu;
        }

        public static MainMenuScreen Create(UiElementFactory uiElementFactory)
        {
            return new(
                new MainMenuScreenController(),
                new ButtonMenu.Builder(s_Container)
                    .Add(s_Option, "New Game", NewGame)
                    .Add(s_Option, "Load Game", LoadGame)
                    .Add(s_Option, "Options", Options)
                    .Build(uiElementFactory));
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Menu!.Draw(target, context);
        }

        public void Initialize()
        {
            Menu!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Menu!.Position = 0.5f * (bounds - Menu!.Size);
            Menu!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Menu!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Menu!.Dispose();
            Menu = null;
        }
    }
}

using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.View.Common.Components;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class MainMenuScreen : ManagedResource, IScreen
    {
        public static readonly object NewGame = new();
        public static readonly object LoadGame = new();
        public static readonly object Options = new();
        public static readonly object Credits = new();

        private static readonly string s_Container = "main-menu-container";
        private static readonly string s_Title = "main-menu-title";
        private static readonly string s_Option = "main-menu-option";

        private static readonly string s_TitleKey = "localize-main-menu-title";
        private static readonly string s_NewGameKey = "localize-main-menu-new";
        private static readonly string s_LoadGameKey = "localize-main-menu-load";
        private static readonly string s_OptionsKey = "localize-main-menu-options";
        private static readonly string s_CreditsKey = "localize-main-menu-credits";

        public IController Controller { get; }
        public TextureBackground? Background { get; private set; }
        public ButtonMenu? Menu { get; private set; }

        private Vector3 _bounds;

        public MainMenuScreen(IController controller, TextureBackground background, ButtonMenu menu)
        {
            Controller = controller;
            Background = background;
            Menu = menu;
        }

        public static MainMenuScreen Create(UiElementFactory uiElementFactory, Localization localization)
        {
            var segment = uiElementFactory.GetTexture("background_main_menu");
            return new(
                new MainMenuScreenController(),
                new TextureBackground(
                    segment.Texture!, segment.TextureView, uiElementFactory.GetShader("shader-default")),
                new ButtonMenu.Builder(s_Container)
                    .SetTitle(localization.Localize(s_TitleKey), s_Title)
                    .Add(s_Option, localization.Localize(s_NewGameKey), NewGame)
                    .Add(s_Option, localization.Localize(s_LoadGameKey), LoadGame)
                    .Add(s_Option, localization.Localize(s_OptionsKey), Options)
                    .Add(s_Option, localization.Localize(s_CreditsKey), Credits)
                    .Build(uiElementFactory));
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Background!.Draw(target, context);

            Menu!.Position = 0.5f * (_bounds - Menu!.Size);
            Menu!.Draw(target, context);
        }

        public void Initialize()
        {
            Menu!.Initialize();
            Background!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            Menu!.ResizeContext(bounds);
            Background!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Menu!.Update(delta);
            Background!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Menu!.Dispose();
            Menu = null;
            Background = null;
        }
    }
}

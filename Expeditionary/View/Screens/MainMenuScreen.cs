using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.View.Common.Components;

namespace Expeditionary.View.Screens
{
    public class MainMenuScreen : BaseScreen
    {
        public static readonly object NewGame = new();
        public static readonly object LoadGame = new();
        public static readonly object Options = new();
        public static readonly object Credits = new();

        private static readonly string ContainerClass = "main-menu-container";
        private static readonly string TitleClass = "main-menu-title";
        private static readonly string OptionClass = "main-menu-option";

        private static readonly string TitleKey = "localize-main-menu-title";
        private static readonly string NewGameKey = "localize-main-menu-new";
        private static readonly string LoadGameKey = "localize-main-menu-load";
        private static readonly string OptionsKey = "localize-main-menu-options";
        private static readonly string CreditsKey = "localize-main-menu-credits";

        public TextureBackground Background { get; private set; }
        public ButtonMenu Menu { get; private set; }

        public MainMenuScreen(IController controller, TextureBackground background, ButtonMenu menu)
            : base(controller)
        {
            Background = background;
            Register(Background);

            Menu = menu;
            Register(Menu);
        }

        public static MainMenuScreen Create(UiElementFactory uiElementFactory, Localization localization)
        {
            var segment = uiElementFactory.GetTexture("background_main_menu");
            return new(
                new MainMenuScreenController(),
                new TextureBackground(
                    segment.Texture!, segment.TextureView, uiElementFactory.GetShader("shader-default")),
                new ButtonMenu.Builder(ContainerClass)
                    .SetTitle(localization.Localize(TitleKey), TitleClass)
                    .Add(OptionClass, localization.Localize(NewGameKey), NewGame)
                    .Add(OptionClass, localization.Localize(LoadGameKey), LoadGame)
                    .Add(OptionClass, localization.Localize(OptionsKey), Options)
                    .Add(OptionClass, localization.Localize(CreditsKey), Credits)
                    .Build(uiElementFactory));
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            Menu!.Position = 0.5f * (_bounds - Menu!.Size);
            base.Draw(target, context);
        }
    }
}

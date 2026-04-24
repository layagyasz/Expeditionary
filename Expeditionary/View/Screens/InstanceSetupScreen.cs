using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.View.Common.Components;

namespace Expeditionary.View.Screens
{
    public class InstanceSetupScreen : BaseScreen
    {
        private static readonly string s_Container = "instance-setup-container";
        private static readonly string s_Title = "instance-setup-title";
        private static readonly string s_Option = "instance-setup-option";

        private static readonly string s_FactionSelectTitleKey = "localize-instance-setup-faction-select-title";

        public TextureBackground? Background { get; private set; }
        public ButtonMenu? FactionSelect { get; private set; }

        public InstanceSetupScreen(IController controller, TextureBackground background, ButtonMenu menu)
            : base(controller)
        {
            Background = background;
            Register(Background);

            FactionSelect = menu;
            Register(FactionSelect);
        }

        public static InstanceSetupScreen Create(
            UiElementFactory uiElementFactory, Localization localization, GameModule module)
        {
            var segment = uiElementFactory.GetTexture("background_main_menu");
            var factionSelect =
                new ButtonMenu.Builder(s_Container).SetTitle(localization.Localize(s_FactionSelectTitleKey), s_Title);
            foreach (var faction in module.Factions.Values.Where(faction => faction.IsPlayable))
            {
                factionSelect.Add(s_Option, faction.Name, faction);
            }
            return new(
                new InstanceSetupScreenController(),
                new TextureBackground(
                    segment.Texture!, segment.TextureView, uiElementFactory.GetShader("shader-default")),
                factionSelect.Build(uiElementFactory));
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            FactionSelect!.Position = 0.5f * (_bounds - FactionSelect!.Size);
            base.Draw(target, context);
        }
    }
}

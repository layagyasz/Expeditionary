using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Screens;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Reporting;
using Expeditionary.View.Common.Components;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class MatchSummaryScreen : BaseScreen
    {
        private static readonly string ContainerClass = "match-summary-container";
        private static readonly string TeamContainerClass = "match-summary-team-container";
        private static readonly string TeamTitleClass = "match-summary-team-title";
        private static readonly string ContinueClass = "match-summary-continue";

        private static readonly string AlliesKey = "localization-match-summary-allies";
        private static readonly string EnemiesKey = "localization-match-summary-enemies";
        private static readonly string ContinueKey = "localization-match-summary-continue";

        private static readonly Vector3 ContinueOffset = new(-16f, -16f, 0);

        public TextureBackground Background { get; private set; }
        public IUiContainer Summary { get; }
        public IUiElement ContinueButton { get; }

        private MatchSummaryScreen(
            IController controller, TextureBackground background, IUiContainer summary, IUiElement continueButton) 
            : base(controller)
        {
            Background = background;
            Register(background);

            Summary = summary;
            Register(summary);

            ContinueButton = continueButton;
            Register(continueButton);
        }

        public static MatchSummaryScreen Create(
            UiElementFactory uiElementFactory, Localization localization, MatchPlayer player, MatchReport report)
        {
            var container =
                new UiSerialContainer(
                    uiElementFactory.GetClass(ContainerClass),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    CreateSideSummary(
                        uiElementFactory,
                        localization,
                        AlliesKey,
                        report.Players
                            .Where(reportPlayer => reportPlayer.Key.Team == player.Team)
                            .Select(kvp => kvp.Value)),
                    CreateSideSummary(
                        uiElementFactory, 
                        localization, 
                        EnemiesKey,
                        report.Players
                            .Where(reportPlayer => reportPlayer.Key.Team != player.Team)
                            .Select(kvp => kvp.Value))
                };
            (var continueButton, var _) = 
                uiElementFactory.CreateTextButton(ContinueClass, localization.Localize(ContinueKey));
            var segment = uiElementFactory.GetTexture("background_main_menu");
            return new(
                new MatchSummaryScreenController(),
                new TextureBackground(
                    segment.Texture!, segment.TextureView, uiElementFactory.GetShader("shader-default")), 
                container, 
                continueButton);
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Summary!.Position = 0.5f * (bounds - Summary!.Size);
            ContinueButton!.Position = bounds - ContinueButton!.Size - ContinueOffset;
        }

        private static IUiContainer CreateSideSummary(
            UiElementFactory uiElementFactory, 
            Localization localization, 
            string titleKey, 
            IEnumerable<PlayerReport> reports)
        {
            var container = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(TeamContainerClass),
                    new TableController(uiElementFactory.GetAudioPlayer(), 1f),
                    UiSerialContainer.Orientation.Vertical);
            var title =
                new TextUiElement(
                    uiElementFactory.GetClass(TeamTitleClass),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    localization.Localize(titleKey));
            container.Add(title);
            return container;
        }
    }
}

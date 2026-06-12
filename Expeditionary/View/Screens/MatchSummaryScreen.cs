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
        private static readonly int GroupEchelon = 3;

        private static readonly string ContainerClass = "match-summary-container";

        private static readonly string TeamContainerClass = "match-summary-team-container";
        private static readonly string TeamTitleClass = "match-summary-team-title";
        private static readonly string TeamTableClass = "match-summary-team-table";

        private static readonly string FormationContainerClass = "match-summary-team-formation-container";
        private static readonly string FormationTitleClass = "match-summary-team-formation-title";
        private static readonly string FormationTableClass = "match-summary-team-formation-table";
        private static readonly string FormationRowClass = "match-summary-team-formation-row";

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

            var table =
                new UiSerialContainer(
                    uiElementFactory.GetClass(TeamTableClass),
                    new TableController(uiElementFactory.GetAudioPlayer(), 10f),
                    UiSerialContainer.Orientation.Vertical);
            container.Add(table);

            foreach (var report in reports)
            {
                foreach (var formationSummary in CreatePlayerSummary(uiElementFactory, report))
                {
                    table.Add(formationSummary);
                }
            }

            return container;
        }

        private static IEnumerable<IUiContainer> CreatePlayerSummary(
            UiElementFactory uiElementFactory, PlayerReport report)
        {
            return report.Formation.GetComponentFormationsBelow(GroupEchelon)
                .Select(formation => CreateFormationSummary(uiElementFactory, formation));
        }

        private static IUiContainer CreateFormationSummary(UiElementFactory uiElementFactory, FormationReport report)
        {
            var container =
                new UiSerialContainer(
                    uiElementFactory.GetClass(FormationContainerClass),
                    new TableController(uiElementFactory.GetAudioPlayer(), 1f),
                    UiSerialContainer.Orientation.Vertical);

            var title =
                new TextUiElement(
                    uiElementFactory.GetClass(FormationTitleClass),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    report.Name);
            container.Add(title);

            var table =
                new UiSerialContainer(
                    uiElementFactory.GetClass(FormationTableClass),
                    new TableController(uiElementFactory.GetAudioPlayer(), 1f),
                    UiSerialContainer.Orientation.Vertical);
            container.Add(table);

            foreach (var unit in report.GetUnits()
                .Where(unit => unit.Number < unit.Type.Intrinsics.Number.GetValue())
                .OrderByDescending(unit => unit.Type.Points))
            {
                table.Add(uiElementFactory.CreateTextButton(FormationRowClass, unit.Name).Item1);
            }

            return container;
        }
    }
}

using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Galaxies;
using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Instances;
using Expeditionary.View.Common;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class MissionPane : UiCompoundComponent, IPane
    {
        private static readonly string ContainerClass = "galaxy-mission-pane";
        private static readonly string ContentsClass = "galaxy-mission-pane-contents";
        private static readonly string CloseButtonClass = "galaxy-mission-pane-close";
        private static readonly string LaunchButtonClass = "galaxy-mission-pane-launch";
        private static readonly string SectionClass = "galaxy-mission-pane-section";
        private static readonly string SectionTitleClass = "galaxy-mission-pane-section-title";
        private static readonly string SectionContentsClass = "galaxy-mission-pane-section-contents";
        private static readonly string SectionLineClass = "galaxy-mission-pane-section-p";
        private static readonly string TitleClass = "galaxy-mission-pane-title";

        private static readonly string EnvironmentKey = "localize-galaxy-mission-pane-environment";
        private static readonly string LaunchKey = "localize-galaxy-mission-pane-launch";
        private static readonly string LocationKey = "localize-galaxy-mission-pane-location";

        public IUiElement CloseButton { get; }
        public IUiElement LaunchButton { get; }

        private readonly TextUiElement _title;
        private readonly IUiContainer _contents;
        private readonly UiElementFactory _uiElementFactory;
        private readonly Localization _localization;

        private MissionPane(
            IController controller,
            UiElementFactory uiElementFactory,
            Localization localization,
            IUiContainer container,
            TextUiElement title, 
            IUiContainer contents,
            IUiElement closeButton,
            IUiElement launchButton)
            : base(controller, container)
        {
            _uiElementFactory = uiElementFactory;
            _localization = localization;
            _title = title;
            _contents = contents;
            CloseButton = closeButton;
            LaunchButton = launchButton;

            Add(_title);

            contents.Position = new Vector3(0, _title.Size.Y, 0);
            Add(contents);

            closeButton.Position = new Vector3(Size.X - _title.RightPadding.X - closeButton.Size.X, 0, 0);
            Add(closeButton);

            launchButton.Position = new(0, _title.Size.Y, 0);
            contents.Add(launchButton);
        }

        public static MissionPane Create(UiElementFactory uiElementFactory, Localization localization)
        {
            return new(
                new MissionPaneController(),
                uiElementFactory,
                localization,
                new UiContainer(
                    uiElementFactory.GetClass(ContainerClass), new PaneController(uiElementFactory.GetAudioPlayer())), 
                new TextUiElement(
                    uiElementFactory.GetClass(TitleClass), 
                    new InlayController(uiElementFactory.GetAudioPlayer()), 
                    string.Empty),
                new UiSerialContainer(
                    uiElementFactory.GetClass(ContentsClass), 
                    new TableController(uiElementFactory.GetAudioPlayer(), 10f), 
                    UiSerialContainer.Orientation.Vertical),
                new SimpleUiElement(
                    uiElementFactory.GetClass(CloseButtonClass), 
                    new SimpleElementController(uiElementFactory.GetAudioPlayer())),
                new TextUiElement(
                    uiElementFactory.GetClass(LaunchButtonClass),
                    new SimpleElementController(uiElementFactory.GetAudioPlayer()),
                    localization.Localize(LaunchKey)))
            {
                Visible = false
            };
        }

        public void SetMission(InstanceMission mission, SectorNaming naming)
        {
            _title.SetText(_localization.Localize(mission.Mission.NameKey));

            // Ensure the launch button is not disposed.
            _contents.Remove(LaunchButton, dispose: false);
            _contents.Clear(dispose: true);

            _contents.Add(
                CreateSection(
                    _uiElementFactory, 
                    _localization.Localize(LocationKey),
                    GenerateLocationContents(_uiElementFactory, _localization, mission, naming)));
            _contents.Add(
                CreateSection(
                    _uiElementFactory,
                    _localization.Localize(EnvironmentKey), 
                    GenerateEnvironmentContents(_uiElementFactory, _localization, mission)));
            _contents.Add(LaunchButton);
        }

        private static IUiContainer CreateSection(
            UiElementFactory uiElementFactory, string title, IEnumerable<IUiElement> elements)
        {
            var container =
                new UiSerialContainer(
                    uiElementFactory.GetClass(SectionClass),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical);
            var header =
                new TextUiElement(
                    uiElementFactory.GetClass(SectionTitleClass), 
                    new InlayController(uiElementFactory.GetAudioPlayer()), 
                    title);
            var contents =
                new UiSerialContainer(
                    uiElementFactory.GetClass(SectionContentsClass),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical);

            foreach (var element in elements)
            {
                contents.Add(element);
            }

            container.Add(header);
            container.Add(contents);
            container.Initialize();

            return container;
        }

        private static IEnumerable<IUiElement> GenerateLocationContents(
            UiElementFactory uiElementFactory, Localization localization, InstanceMission mission, SectorNaming naming)
        {
            yield return new TextUiElement(
                uiElementFactory.GetClass(SectionLineClass),
                new InlayController(uiElementFactory.GetAudioPlayer()),
                naming.Name(mission.Mission.Map.Environment));
        }

        private static IEnumerable<IUiElement> GenerateEnvironmentContents(
            UiElementFactory uiElementFactory, Localization localization, InstanceMission mission)
        {
            foreach (var trait in mission.Mission.Map.Environment.Traits)
            {
                string text = localization.Localize("localize-" + trait.Key);
                if (text.Any())
                {
                    yield return new TextUiElement(
                        uiElementFactory.GetClass(SectionLineClass),
                        new InlayController(uiElementFactory.GetAudioPlayer()),
                        text);
                }
            }
        }
    }
}

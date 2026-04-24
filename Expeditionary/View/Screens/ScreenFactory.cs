using Cardamom.Ui;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Matches;
using Expeditionary.Spectra;
using Expeditionary.View.Common.Components;
using Expeditionary.View.Scenes;
using Expeditionary.View.Scenes.Galaxies;
using Expeditionary.View.Scenes.Matches.Overlays;

namespace Expeditionary.View.Screens
{
    public class ScreenFactory
    {
        private readonly UiElementFactory _uiElementFactory;
        private readonly SceneFactory _sceneFactory;
        private readonly Localization _localization;
        private readonly SpectrumSensitivity _spectrumSensitivity;

        public ScreenFactory(
            UiElementFactory uiElementFactory,
            SceneFactory sceneFactory,
            Localization localization, 
            SpectrumSensitivity spectrumSensitivity)
        {
            _uiElementFactory = uiElementFactory;
            _sceneFactory = sceneFactory;
            _localization = localization;
            _spectrumSensitivity = spectrumSensitivity;
        }

        public GalaxyScreen CreateGalaxy(GameModule module, GameInstance instance)
        {
            return new GalaxyScreen(
                new GalaxyScreenController(module.SectorNamings.First().Value),
                _sceneFactory.Create(module.Galaxy, instance),
                MissionPane.Create(_uiElementFactory));
        }

        public InstanceSetupScreen CreateInstanceSetup(GameModule module)
        {
            return InstanceSetupScreen.Create(_uiElementFactory, _localization, module);
        }

        public LoadScreen CreateLoad(ILoaderTask task, LoaderStatus status)
        {
            return LoadScreen.Create(_uiElementFactory, task, status);
        }

        public MainMenuScreen CreateMainMenu()
        {
            return MainMenuScreen.Create(_uiElementFactory, _localization);
        }

        public MatchScreen CreateMatch(Match match, MapAppearance appearance, MatchPlayer player)
        {
            return new MatchScreen(
                new MatchController(match, player),
                _sceneFactory.Create(match, appearance.Materialize(_spectrumSensitivity), seed: 0),
                FinishedOverlay.Create(_uiElementFactory, _localization),
                ObjectiveOverlay.Create(_uiElementFactory, _localization, match, player),
                new UnitOverlay(_uiElementFactory),
                RightClickMenu.Create(_uiElementFactory));
        }
    }
}

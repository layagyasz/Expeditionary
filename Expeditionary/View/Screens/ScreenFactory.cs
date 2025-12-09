using Cardamom.Ui;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Missions;
using Expeditionary.Spectra;
using Expeditionary.View.Common.Components;
using Expeditionary.View.Scenes;
using Expeditionary.View.Scenes.Galaxies;
using Expeditionary.View.Scenes.Matches;

namespace Expeditionary.View.Screens
{
    public class ScreenFactory
    {
        private readonly UiElementFactory _uiElementFactory;
        private readonly SceneFactory _sceneFactory;
        private readonly SpectrumSensitivity _spectrumSensitivity;

        public ScreenFactory(
            UiElementFactory uiElementFactory, SceneFactory sceneFactory, SpectrumSensitivity spectrumSensitivity)
        {
            _uiElementFactory = uiElementFactory;
            _sceneFactory = sceneFactory;
            _spectrumSensitivity = spectrumSensitivity;
        }

        public GalaxyScreen CreateGalaxy(GameModule module, MissionManager missionManager)
        {
            return new GalaxyScreen(
                new GalaxyScreenController(module.SectorNamings.First().Value),
                _sceneFactory.Create(module.Galaxy, missionManager),
                MissionPane.Create(_uiElementFactory));
        }

        public LoadScreen CreateLoad(ILoaderTask task, LoaderStatus status)
        {
            return LoadScreen.Create(_uiElementFactory, task, status);
        }

        public MainMenuScreen CreateMainMenu()
        {
            return MainMenuScreen.Create(_uiElementFactory);
        }

        public MatchScreen CreateMatch(Match match, MapAppearance appearance, Player player)
        {
            return new MatchScreen(
                new MatchController(match, player),
                _sceneFactory.Create(match, appearance.Materialize(_spectrumSensitivity), seed: 0),
                new UnitOverlay(_uiElementFactory),
                RightClickMenu.Create(_uiElementFactory));
        }
    }
}

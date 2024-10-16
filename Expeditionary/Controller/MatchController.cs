using Cardamom.Ui.Controller;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Model;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Factions;
using Expeditionary.View;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller
{
    public class MatchController : IController
    {
        private readonly Match _match;
        private readonly Faction _faction;

        private MatchScreen? _screen;
        private MatchSceneController? _controller;

        private Unit? _selectedUnit;

        public MatchController(Match match, Faction faction)
        {
            _match = match;
            _faction = faction;
        }

        public void Bind(object @object)
        {
            _screen = (MatchScreen)@object;
            _controller = (MatchSceneController)_screen.Scene!.Controller;
            _controller.AssetClicked += HandleAssetClicked;
            _controller.HexClicked += HandleHexClicked;
        }

        public void Unbind()
        {
            _controller!.AssetClicked -= HandleAssetClicked;
            _controller!.HexClicked -= HandleHexClicked;
        }

        private void HandleAssetClicked(object? sender, AssetClickedEventArgs e)
        {
            if (e.Button.Button == MouseButton.Left)
            {
                var units = e.Assets.Where(x => x is Unit).Cast<Unit>().Where(x => x.Faction == _faction).ToList();
                int index = _selectedUnit == null ? -1 : units.IndexOf(_selectedUnit);
                if (index == -1)
                {
                    _selectedUnit = units.First();
                }
                else
                {
                    _selectedUnit = units[(index + 1) % units.Count];
                }
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {

        }
    }
}

using Cardamom.Mathematics;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Model;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Factions;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller
{
    public class MatchController : IController
    {
        private readonly Match _match;
        private readonly Faction _faction;

        private MatchScreen? _screen;
        private HighlightLayer? _highlightLayer;
        private MatchSceneController? _sceneController;
        private UnitOverlayController? _unitOverlayController;

        private Unit? _selectedUnit;
        private ButtonId _selectedOrder;

        public MatchController(Match match, Faction faction)
        {
            _match = match;
            _faction = faction;
        }

        public void Bind(object @object)
        {
            _screen = (MatchScreen)@object;

            _highlightLayer = _screen.Scene!.Highlight;

            _sceneController = (MatchSceneController)_screen.Scene!.Controller;
            _sceneController.AssetClicked += HandleAssetClicked;
            _sceneController.HexClicked += HandleHexClicked;

            _unitOverlayController = _screen.UnitOverlay!.ComponentController as UnitOverlayController;
            _unitOverlayController!.OrderChanged += HandleOrderChanged;

            _screen.UnitOverlay.Visible = false;
        }

        public void Unbind()
        {
            _sceneController!.AssetClicked -= HandleAssetClicked;
            _sceneController!.HexClicked -= HandleHexClicked;
            _sceneController = null;

            _unitOverlayController = null;
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
                UpdateUnitOverlay();
                UpdateOrder();
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {

        }

        private void HandleOrderChanged(object? sender, EventArgs e)
        {
            UpdateOrder();
        }

        private void UpdateUnitOverlay()
        {
            if (_selectedUnit != null)
            {
                _screen!.UnitOverlay!.Visible = true;
                _unitOverlayController!.SetOrder(ButtonId.Attack);
            }
            else
            {
                _screen!.UnitOverlay!.Visible = false;
                _highlightLayer!.SetHighlight(Enumerable.Empty<HighlightLayer.HexHighlight>());
            }
        }

        private void UpdateOrder()
        {
            _selectedOrder = _unitOverlayController!.GetOrder();
            if (_selectedUnit != null)
            {
                if (_selectedOrder == ButtonId.Attack)
                {
                    var range = (int)_selectedUnit.Type.Attack.First().Range.GetValue();
                    _highlightLayer!.SetHighlight(
                        Sighting.GetSightField(_match.GetMap(), _selectedUnit.Position, range)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Target, HighlightLayer.GetLevel(x.Distance, new Interval(0, range)))));
                }
                else if (_selectedOrder == ButtonId.Move)
                {
                    var movement = (int)_selectedUnit.Type.Speed;
                    _highlightLayer!.SetHighlight(
                        Pathing.GetPathField(
                            _match.GetMap(), _selectedUnit.Position, movement, _selectedUnit.Type.Movement)
                        .Select(x => new HighlightLayer.HexHighlight(
                            x.Destination, HighlightLayer.GetLevel(x.Cost, new Interval(0, movement)))));
                }
            }
        }
    }
}

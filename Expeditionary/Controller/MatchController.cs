using Cardamom.Mathematics;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Mapping;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Orders;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller
{
    public class MatchController : IController
    {
        private readonly GameDriver _driver;
        private readonly Player _player;

        private MatchScreen? _screen;
        private HighlightLayer? _highlightLayer;
        private MatchSceneController? _sceneController;
        private UnitOverlayController? _unitOverlayController;

        private Unit? _selectedUnit;
        private ButtonId _selectedOrder;

        public MatchController(GameDriver driver, Player player)
        {
            _driver = driver;
            _player = player;
        }

        public void Bind(object @object)
        {
            _screen = (MatchScreen)@object;

            _highlightLayer = _screen.Scene!.Highlight;

            _sceneController = (MatchSceneController)_screen.Scene!.Controller;
            _sceneController.AssetClicked += HandleAssetClicked;
            _sceneController.HexClicked += HandleHexClicked;
            _sceneController.SetPlayer(_player);

            _unitOverlayController = _screen.UnitOverlay!.ComponentController as UnitOverlayController;
            _unitOverlayController!.OrderChanged += HandleOrderChanged;

            _driver.GetMatch().AssetMoved += HandleAssetMoved;

            _screen.UnitOverlay.Visible = false;
        }

        public void Unbind()
        {
            _sceneController!.AssetClicked -= HandleAssetClicked;
            _sceneController!.HexClicked -= HandleHexClicked;
            _sceneController = null;

            _unitOverlayController!.OrderChanged -= HandleOrderChanged;
            _unitOverlayController = null;

            _driver.GetMatch().AssetMoved -= HandleAssetMoved;
        }

        private void HandleAssetClicked(object? sender, AssetClickedEventArgs e)
        {
            if (e.Button.Button == MouseButton.Left)
            {
                var units = e.Assets.Where(x => x is Unit).Cast<Unit>().Where(x => x.Player == _player).ToList();
                if (units.Count == 0)
                {
                    return;
                }
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
            else if (e.Button.Button == MouseButton.Right 
                && _selectedOrder == ButtonId.Attack 
                && _selectedUnit != null
                && e.Assets.First() is Unit defender)
            {
                _driver.DoOrder(new AttackOrder(_selectedUnit, _selectedUnit.Type.Attacks.First(), defender));
            }
        }

        private void HandleAssetMoved(object? sender, AssetMovedEventArgs e)
        {
            if (e.Asset == _selectedUnit && _selectedUnit != null)
            {
                UpdateOrder();
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            if (e.Button.Button == MouseButton.Right && _selectedOrder == ButtonId.Move && _selectedUnit != null)
            {
                // Cheap check to make sure hex is reachable
                if (Geometry.GetCubicDistance(e.Hex, _selectedUnit.Position) <= _selectedUnit.Movement)
                {
                    _driver.DoOrder(
                        new MoveOrder(
                            _selectedUnit, 
                            Pathing.GetShortestPath(
                                _driver.GetMatch().GetMap(),
                                _selectedUnit.Position,
                                e.Hex,
                                _selectedUnit.Type.Movement, 
                                _selectedUnit.Type.Speed)));
                }
            }
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
                _unitOverlayController!.SetUnit(_selectedUnit);
                _unitOverlayController!.SetOrder(ButtonId.Attack);
            }
            else
            {
                _screen!.UnitOverlay!.Visible = false;
                _unitOverlayController!.SetUnit(null);
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
                    var range = (int)_selectedUnit.Type.Attacks.First().Range.GetValue();
                    _highlightLayer!.SetHighlight(
                        Sighting.GetSightField(_driver.GetMatch().GetMap(), _selectedUnit.Position, range)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Target, HighlightLayer.GetLevel(x.Distance, new Interval(0, range)))));
                }
                else if (_selectedOrder == ButtonId.Move)
                {
                    var movement = (int)_selectedUnit.Type.Speed;
                    var used = _selectedUnit.Type.Speed - _selectedUnit.Movement;
                    _highlightLayer!.SetHighlight(
                        Pathing.GetPathField(
                            _driver.GetMatch().GetMap(), 
                            _selectedUnit.Position, 
                            _selectedUnit.Type.Movement, 
                            _selectedUnit.Movement)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Destination, HighlightLayer.GetLevel(x.Cost + used, new Interval(0, movement)))));
                }
            }
        }
    }
}

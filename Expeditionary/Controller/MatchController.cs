using Cardamom.Logging;
using Cardamom.Mathematics;
using Cardamom.Ui.Controller;
using Cardamom.Window;
using Expeditionary.Controller.Mapping;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller
{
    public class MatchController : IController
    {
        private static readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(MatchController));

        private readonly Match _match;
        private readonly Player _player;

        private MatchScreen? _screen;
        private HighlightLayer? _highlightLayer;
        private MatchSceneController? _sceneController;
        private UnitOverlayController? _unitOverlayController;
        private IFormFieldController<object>? _unitSelectController;

        private Unit? _selectedUnit;
        private OrderValue? _selectedOrder;
        private IEnumerator<Unit>? _selectedUnitEnumerator;

        public MatchController(Match match, Player player)
        {
            _match = match;
            _player = player;
        }

        public void Bind(object @object)
        {
            _screen = (MatchScreen)@object;

            _match.Stepped += HandleStep;

            _highlightLayer = _screen.Scene!.Highlight;

            _sceneController = (MatchSceneController)_screen.Scene!.Controller;
            _sceneController.AssetClicked += HandleAssetClicked;
            _sceneController.HexClicked += HandleHexClicked;
            _sceneController.TextEntered += HandleTextEntered;
            _sceneController.SetPlayer(_player);

            _unitOverlayController = _screen.UnitOverlay!.ComponentController as UnitOverlayController;
            _unitOverlayController!.OrderChanged += HandleOrderChanged;
            _unitOverlayController.SetMatch(_match);

            _unitSelectController = _screen.UnitSelect!.ComponentController as IFormFieldController<object>;
            _unitSelectController!.ValueChanged += HandleUnitSelected;

            _screen.UnitOverlay.Visible = false;
            _screen.Updated += HandleFrame;
        }

        public void Unbind()
        {
            _match.Stepped -= HandleStep;

            _sceneController!.AssetClicked -= HandleAssetClicked;
            _sceneController!.HexClicked -= HandleHexClicked;
            _sceneController!.TextEntered -= HandleTextEntered;
            _sceneController = null;

            _unitOverlayController!.OrderChanged -= HandleOrderChanged;
            _unitOverlayController = null;

            _unitSelectController!.ValueChanged -= HandleUnitSelected;
            _unitSelectController = null;

            _screen!.Updated -= HandleFrame;
        }

        private void HandleFrame(object? sender, EventArgs e)
        {
            _match.DispatchEvents();
        }

        private void HandleAssetClicked(object? sender, AssetClickedEventArgs e)
        {
            CloseRightClickMenu();
            if (e.Button.Button == MouseButton.Left)
            {
                SetSelectedUnit(
                    e.Assets.Where(x => x is Unit).Cast<Unit>().Where(x => x.Player == _player).FirstOrDefault());
            }
            else if (e.Button.Button == MouseButton.Right)
            {
                if (_selectedOrder?.OrderId == OrderId.Attack
                    && _selectedUnit != null
                    && e.Assets.First() is Unit defender)
                {
                    var weapon = (UnitWeaponUsage)_selectedOrder.Args![0];
                    var mode = (UnitWeapon.Mode)_selectedOrder.Args![1];
                    DoOrder(new AttackOrder(_selectedUnit, weapon, mode, defender));
                }
                else
                {
                    OpenRightClickMenu(
                        e.Button.ScreenPosition, 
                        e.Assets.Where(x => x is Unit).Cast<Unit>().Where(x => x.Player == _player));
                }
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            s_Logger.Log(_match.GetMap().Get(e.Hex)?.ToString() ?? "off map");
            CloseRightClickMenu();
            if (e.Button.Button == MouseButton.Right
                && _selectedOrder?.OrderId == OrderId.Move
                && _selectedUnit != null)
            {
                // Cheap check to make sure hex is reachable
                if (Geometry.GetCubicDistance(e.Hex, _selectedUnit.Position!.Value) <= _selectedUnit.Type.Speed)
                {
                    DoOrder(
                        new MoveOrder(
                            _selectedUnit,
                            Pathing.GetShortestPath(
                                _match.GetMap(),
                                _selectedUnit.Position.Value,
                                e.Hex,
                                _selectedUnit.Type.Movement,
                                TileConsiderations.None,
                                _selectedUnit.Type.Speed)));
                }
            }
        }

        private void HandleTextEntered(object? sender, TextEnteredEventArgs e)
        {
            if (e.Key == Keys.Space)
            {
                StepSelectedUnit();
            }
        }

        private void HandleOrderChanged(object? sender, EventArgs e)
        {
            UpdateOrder();
        }

        private void HandleStep(object? sender, EventArgs e)
        {
            if (_player == _match.GetActivePlayer())
            {
                StepSelectedUnit();
            }
        }
        
        private void HandleUnitSelected(object? sender, EventArgs e)
        {
            if (_unitSelectController!.GetValue() is Unit unit)
            {
                SetSelectedUnit(unit);
            }
            CloseRightClickMenu();
        }

        private void DoOrder(IOrder order)
        {
            if (_match.DoOrder(order))
            {
                StepSelectedUnit();
            }
        }

        private void CloseRightClickMenu()
        {
            _screen!.UnitSelect!.Visible = false;
        }

        private void OpenRightClickMenu(Vector2 position, IEnumerable<object> options)
        {
            _screen!.UnitSelect!.Set(options.Select(x => SelectOption<object>.Create(x, GetObjectName(x))));
            _screen!.UnitSelect!.Position = new(position.X, position.Y, 0);
            _screen!.UnitSelect!.Visible = true;
        }

        private void SetSelectedUnit(Unit? unit)
        {
            _selectedUnit = unit;
            _selectedUnitEnumerator = null;
            UpdateUnitOverlay();
            UpdateOrder();
        }

        private void StepSelectedUnit()
        {
            if (_selectedUnitEnumerator == null)
            {
                _selectedUnitEnumerator = StepActiveUnitEnumerator();
                if (_selectedUnit != null)
                {
                    while (_selectedUnitEnumerator.MoveNext() && _selectedUnit != _selectedUnitEnumerator.Current) ;
                }
            }
            _selectedUnit = null;
            while (_selectedUnitEnumerator.MoveNext() && !IsUnitActionable(_selectedUnitEnumerator.Current)) ;
            _selectedUnit = _selectedUnitEnumerator.Current;
            // Could not find next unit. Reset enumerator and try again from the beginning.  If there is still no
            // candidate unit found, then there are no more actions to be taken.
            if (_selectedUnit == null)
            {
                _selectedUnitEnumerator = StepActiveUnitEnumerator();
                while (_selectedUnitEnumerator.MoveNext() && !IsUnitActionable(_selectedUnitEnumerator.Current)) ;
                _selectedUnit = _selectedUnitEnumerator.Current;
            }
            UpdateUnitOverlay();
        }

        private IEnumerator<Unit> StepActiveUnitEnumerator()
        {
            return _match.GetFormations(_player)
                .SelectMany(x => x.GetDiads())
                .SelectMany(x => x.GetUnits())
                .GetEnumerator();
        }

        private void UpdateUnitOverlay()
        {
            if (_selectedUnit != null)
            {
                _screen!.UnitOverlay!.Visible = true;
                _unitOverlayController!.SetUnit(_selectedUnit);
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
                if (_selectedOrder?.OrderId == OrderId.Attack)
                {
                    var mode = (UnitWeapon.Mode)_selectedOrder.Args[1];
                    var range = (int)mode.Range.GetMaximum();
                    _highlightLayer!.SetHighlight(
                        CombatCalculator.GetValidAttackHexes(mode, _match.GetMap(), _selectedUnit.Position!.Value)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Target, HighlightLayer.GetLevel(x.Distance, new Interval(0, range)))));
                }
                else if (_selectedOrder?.OrderId == OrderId.Move)
                {
                    var movement = (int)_selectedUnit.Type.Speed;
                    var used = _selectedUnit.Type.Speed - _selectedUnit.Type.Speed;
                    _highlightLayer!.SetHighlight(
                        Pathing.GetPathField(
                            _match.GetMap(),
                            _selectedUnit.Position!.Value,
                            _selectedUnit.Type.Movement,
                            TileConsiderations.None,
                            _selectedUnit.Type.Speed)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Destination, HighlightLayer.GetLevel(x.Cost + used, new Interval(0, movement)))));
                }
                else if (_selectedOrder?.OrderId == OrderId.Load)
                {
                    DoOrder(new LoadOrder(_selectedUnit, (IAsset)_selectedOrder.Args[0]));
                }
                else if (_selectedOrder?.OrderId == OrderId.Unload) 
                {
                    DoOrder(new UnloadOrder(_selectedUnit));
                }
            }
        }

        private static string GetObjectName(object @object)
        {
            if (@object is Unit unit)
            {
                return unit.Name;
            }
            return @object?.ToString() ?? string.Empty;
        }

        private static bool IsUnitActionable(Unit? unit)
        {
            return unit != null && unit.IsActive() && unit.Actions > 0 && !unit.IsPassenger;
        }
    }
}

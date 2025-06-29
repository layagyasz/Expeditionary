﻿using Cardamom.Logging;
using Cardamom.Mathematics;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Mapping;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches;
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

        private Unit? _selectedUnit;
        private ButtonId _selectedOrder;

        public MatchController(Match match, Player player)
        {
            _match = match;
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

            _screen.UnitOverlay.Visible = false;
            _screen.Updated += HandleFrame;
        }

        public void Unbind()
        {
            _sceneController!.AssetClicked -= HandleAssetClicked;
            _sceneController!.HexClicked -= HandleHexClicked;
            _sceneController = null;

            _unitOverlayController!.OrderChanged -= HandleOrderChanged;
            _unitOverlayController = null;

            _screen!.Updated -= HandleFrame;
        }

        private void HandleFrame(object? sender, EventArgs e)
        {
            _match.DispatchEvents();
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
                var weapon = _selectedUnit.Type.Weapons.First();
                var mode = weapon.Weapon!.Modes.First();
                _match.DoOrder(new AttackOrder(_selectedUnit, weapon, mode, defender));
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            s_Logger.Log(_match.GetMap().Get(e.Hex)?.ToString() ?? "off map");
            if (e.Button.Button == MouseButton.Right && _selectedOrder == ButtonId.Move && _selectedUnit != null)
            {
                // Cheap check to make sure hex is reachable
                if (Geometry.GetCubicDistance(e.Hex, _selectedUnit.Position!.Value) <= _selectedUnit.Movement)
                {
                    _match.DoOrder(
                        new MoveOrder(
                            _selectedUnit, 
                            Pathing.GetShortestPath(
                                _match.GetMap(),
                                _selectedUnit.Position.Value,
                                e.Hex,
                                _selectedUnit.Type.Movement,
                                TileConsiderations.None,
                                _selectedUnit.Type.Speed)));
                    UpdateOrder();
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
                _highlightLayer!.SetHighlight(
                    HighlightLayer.ForConsideration(
                        _match.GetMap(),
                        TileConsiderations.Threat(_selectedUnit, _match.GetKnowledge(_player), _match)));
                /*
                if (_selectedOrder == ButtonId.Attack)
                {
                    var range = 
                        (int)_selectedUnit.Type.Weapons
                            .SelectMany(x => x.Weapon!.Modes).Select(x => x.Range.Get()).Max();
                    _highlightLayer!.SetHighlight(
                        Sighting.GetUnblockedSightField(_match.GetMap(), _selectedUnit.Position!.Value, range)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Target, HighlightLayer.GetLevel(x.Distance, new Interval(0, range)))));
                }
                else if (_selectedOrder == ButtonId.Move)
                {
                    var movement = (int)_selectedUnit.Type.Speed;
                    var used = _selectedUnit.Type.Speed - _selectedUnit.Movement;
                    _highlightLayer!.SetHighlight(
                        Pathing.GetPathField(
                            _match.GetMap(), 
                            _selectedUnit.Position!.Value, 
                            _selectedUnit.Type.Movement, 
                            TileConsiderations.None,
                            _selectedUnit.Movement)
                            .Select(x => new HighlightLayer.HexHighlight(
                                x.Destination, HighlightLayer.GetLevel(x.Cost + used, new Interval(0, movement)))));
                }
                */
            }
        }
    }
}

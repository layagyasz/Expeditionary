﻿using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Window;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.View;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Expeditionary.Controller
{
    public class AssetLayerController : IElementController
    {
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        private AssetLayer? _assetLayer;

        private readonly MultiMap<Vector3i, IAsset> _positionMap = new();

        public void Bind(object @object)
        {
            _assetLayer = ((InteractiveModel)@object).GetModel() as AssetLayer;
        }

        public void Unbind()
        {
            _assetLayer = null;
        }

        public void AddAsset(IAsset asset)
        {
            _positionMap.Add(asset.Position, asset);
            _assetLayer!.Add(asset);
        }

        public void RemoveAsset(IAsset asset)
        {
            _positionMap.Remove(asset.Position, asset);
            _assetLayer!.Remove(asset);
        }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return false;
        }

        public bool HandleMouseEntered()
        {
            return false;
        }

        public bool HandleMouseLeft()
        {
            return false;
        }

        public bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            var hex = Geometry.SnapToHex(Cubic.Cartesian.Instance.Wrap(e.Position.Xz));
            if (_positionMap.TryGetValue(hex, out var assets))
            {
                Console.WriteLine(assets.First());
                return true;
            }
            return false;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return false;
        }

        public bool HandleMouseLingered()
        {
            return false;
        }

        public bool HandleMouseLingerBroken()
        {
            return false;
        }

        public bool HandleFocusEntered()
        {
            return false;
        }

        public bool HandleFocusLeft()
        {
            return false;
        }
    }
}
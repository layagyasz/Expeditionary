using Cardamom.Collections;
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
        private static readonly Vector2 s_Scale = 0.5f * MathF.Sqrt(3) * new Vector2(0.6f, 0.4f);
        private static readonly Box2 s_Bounds = new(-s_Scale, s_Scale);

        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        public EventHandler<AssetClickedEventArgs>? AssetClicked { get; set; }

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

        public void AddAsset(IAsset asset, Vector3i position)
        {
            _positionMap.Add(position, asset);
            _assetLayer!.Add(asset, position);
        }

        public void RemoveAsset(IAsset asset)
        {
            _positionMap.Remove(asset.Position, asset);
            _assetLayer!.Remove(asset);
        }

        public void MoveAsset(IAsset asset, Vector3i origin, Vector3i destination)
        {
            _positionMap.Remove(origin, asset);
            _positionMap.Add(destination, asset);
            _assetLayer!.Move(asset, destination);
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
            var coord = e.Position.Xz - Cubic.Cartesian.Instance.Project(hex);
            if (s_Bounds.ContainsInclusive(coord) && _positionMap.TryGetValue(hex, out var assets))
            {
                AssetClicked?.Invoke(this, new(assets.ToList(), e));
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

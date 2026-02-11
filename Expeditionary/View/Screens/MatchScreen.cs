using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Common.Components;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes.Matches.Overlays;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class MatchScreen : ManagedResource, IScreen
    {
        public IController Controller { get; }
        public MatchScene? Scene { get; private set; }
        public ObjectiveOverlay? ObjectiveOverlay { get; private set; }
        public UnitOverlay? UnitOverlay { get; private set; }
        public RightClickMenu? UnitSelect { get; private set; }

        public MatchScreen(
            IController controller, 
            MatchScene scene, 
            ObjectiveOverlay objectiveOverlay,
            UnitOverlay unitOverlay, 
            RightClickMenu unitSelect)
        {
            Controller = controller;
            Scene = scene;
            ObjectiveOverlay = objectiveOverlay;
            UnitOverlay = unitOverlay;
            UnitSelect = unitSelect;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene!.Draw(target, context);
            context.Flatten();
            ObjectiveOverlay!.Draw(target, context);
            UnitOverlay!.Draw(target, context);
            UnitSelect!.Draw(target, context);
        }

        public void Initialize()
        {
            Scene!.Initialize();
            ObjectiveOverlay!.Initialize();
            UnitOverlay!.Initialize();
            UnitSelect!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Scene!.ResizeContext(bounds);
            ObjectiveOverlay!.ResizeContext(bounds);
            UnitOverlay!.ResizeContext(bounds);
            UnitSelect!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Scene!.Update(delta);
            ObjectiveOverlay!.Update(delta);
            UnitOverlay!.Update(delta);
            UnitSelect!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Scene!.Dispose();
            Scene = null;
            ObjectiveOverlay!.Dispose();
            ObjectiveOverlay = null;
            UnitOverlay!.Dispose();
            UnitOverlay = null;
            UnitSelect!.Dispose();
            UnitSelect = null;
        }
    }
}

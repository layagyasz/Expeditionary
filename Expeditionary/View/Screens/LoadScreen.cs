using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.View.Common.Components;
using Expeditionary.View.Common.Components.Dynamics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class LoadScreen : ManagedResource, IRenderable
    {
        private static readonly long s_RefreshTime = 100;

        public event EventHandler<EventArgs>? Updated;

        public IController Controller { get; }
        public LoadBar LoadBar { get; }

        private readonly DynamicRefresher _refresher = new(s_RefreshTime);

        private LoadScreen(IController controller, LoadBar loadBar)
        {
            Controller = controller;
            LoadBar = loadBar;

            _refresher.Add(LoadBar);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            LoadBar.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            LoadBar.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            LoadBar.Position = 0.5f * (bounds - LoadBar.Size);
        }

        public void Update(long delta)
        {
            _refresher.Update(delta);
            LoadBar.Update(delta);
            Updated?.Invoke(this, EventArgs.Empty);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            LoadBar.Dispose();
        }

        public static LoadScreen Create(
            UiElementFactory uiElementFactory, ILoaderTask loaderTask, LoaderStatus loaderStatus)
        {
            return new LoadScreen(
                new LoadScreenController(loaderTask), LoadBar.Create(uiElementFactory, loaderStatus));
        }
    }
}

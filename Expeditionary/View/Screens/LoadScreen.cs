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
        public TextureBackground Background { get; }
        public LoadBar LoadBar { get; }

        private readonly DynamicRefresher _refresher = new(s_RefreshTime);

        private LoadScreen(IController controller, TextureBackground background, LoadBar loadBar)
        {
            Controller = controller;
            Background = background;
            LoadBar = loadBar;

            _refresher.Add(LoadBar);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Background.Draw(target, context);
            LoadBar.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            Background.Initialize();
            LoadBar.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {

            Background.ResizeContext(bounds);
            LoadBar.Position = 0.5f * (bounds - LoadBar.Size);
            LoadBar.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            _refresher.Update(delta);
            LoadBar.Update(delta);
            Background.Update(delta);
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
            var segment = uiElementFactory.GetTexture("background_main_menu");
            return new LoadScreen(
                new LoadScreenController(loaderTask),
                new TextureBackground(
                    segment.Texture!, segment.TextureView, uiElementFactory.GetShader("shader-default")),
                LoadBar.Create(uiElementFactory, loaderStatus));
        }
    }
}

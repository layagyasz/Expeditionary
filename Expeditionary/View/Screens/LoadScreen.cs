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
    public class LoadScreen : ManagedResource, IDynamic, IScreen
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IController Controller { get; }
        public TextureBackground Background { get; }
        public LoadBar LoadBar { get; }

        private LoadScreen(IController controller, TextureBackground background, LoadBar loadBar)
        {
            Controller = controller;
            Background = background;
            LoadBar = loadBar;
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

        public void Refresh()
        {
            LoadBar.Refresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {

            Background.ResizeContext(bounds);
            LoadBar.Position = 0.5f * (bounds - LoadBar.Size);
            LoadBar.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            LoadBar.Update(delta);
            Background.Update(delta);
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

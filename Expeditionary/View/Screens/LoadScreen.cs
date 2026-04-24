using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.View.Common.Components;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class LoadScreen : BaseScreen
    {
        public TextureBackground Background { get; }
        public LoadBar LoadBar { get; }

        private LoadScreen(IController controller, TextureBackground background, LoadBar loadBar)
            : base(controller)
        {
            Background = background;
            Register(Background);

            LoadBar = loadBar;
            Register(LoadBar);
        }

        public override void Refresh()
        {
            LoadBar.Refresh();
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            LoadBar!.Position = 0.5f * (bounds - LoadBar!.Size);
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

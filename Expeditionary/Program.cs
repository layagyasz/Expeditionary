using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Generation;
using Expeditionary.Model.Mapping;
using Expeditionary.View;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var window = new RenderWindow("Expeditionary", new(512, 512));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            /*
            var tileBaseGenerator = 
                new TileBaseGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/Generation/default.vert")
                        .SetFragment("Resources/Generation/partition.frag").Build());
            tileBaseGenerator.Generate(2f, seed: 0);
            */

            var mapGenerator = new MapGenerator();
            var sceneFactory = 
                new SceneFactory(
                    new MapViewFactory(
                        new RenderShader.Builder()
                            .SetVertex("default.vert")
                            .SetFragment("default_no_tex.frag")
                            .Build()));

            ui.SetRoot(sceneFactory.Create(new(512, 512, 0), mapGenerator.Generate(new(100, 100), seed: 0)));
            ui.Start();
        }
    }
}

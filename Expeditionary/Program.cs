﻿using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Generation;
using Expeditionary.Model.Mapping;
using Expeditionary.View;
using OpenTK.Mathematics;
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

            var tileBaseGenerator = 
                new TerrainTextureGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/Generation/default.vert")
                        .SetFragment("Resources/Generation/partition.frag").Build());
           var tileBases = tileBaseGenerator.Generate(attenuationRange: new(0.5f, 4f), seed: 0, count: 60);

            var mapGenerator = new MapGenerator();
            var sceneFactory = 
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(-0.25f, 0.1f),
                            ElevationLevel = 10
                        },
                        tileBases,
                        new RenderShader.Builder()
                            .SetVertex("default.vert")
                            .SetFragment("default.frag")
                            .Build()));

            ui.SetRoot(
                sceneFactory.Create(
                    new(512, 512, 0), 
                    mapGenerator.Generate(
                        new(),
                        new(100, 100),
                        seed: 0),
                    new() 
                    { 
                        StoneParameters = 
                            new(
                                new Color4[] 
                                {
                                    new(200, 200, 200, 255),
                                    new(240, 240, 240, 255),
                                    new(227, 173, 156, 255)
                                })
                    },
                    seed: 0));
            ui.Start();
        }
    }
}
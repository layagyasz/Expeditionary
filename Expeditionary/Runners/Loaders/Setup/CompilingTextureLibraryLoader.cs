using Cardamom.Graphics;
using Cardamom;
using Expeditionary.View.Textures.Generation;
using Expeditionary.View.Textures;
using OpenTK.Mathematics;
using System.Text.Json;
using Cardamom.Json.OpenTK;
using Cardamom.Json;

namespace Expeditionary.Runners.Loaders.Setup
{
    public class CompilingTextureLibraryLoader
    {
        private readonly GameResources _resources;

        public CompilingTextureLibraryLoader(GameResources resources)
        {
            _resources = resources;
        }

        public MapTextureLibrary Load()
        {
            // TODO: Compile other texture libraries
            var partitionTextureGenerator =
                new PartitionTextureGenerator(_resources.GetShader("shader-generate-partition"));
            var partitions =
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 4f), seed: 0, count: 60);

            var maskTextureGenerator = new FoliageTextureGenerator(_resources.GetShader("shader-generate-mask"));
            var masks = maskTextureGenerator.Generate(
                frequencyRange: new(4f, 8f), magnitudeRange: new(0f, 1f), seed: 0, count: 16);

            var edgeTextureGenerator = new EdgeTextureGenerator(_resources.GetShader("shader-generate-edge"));
            var rivers = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 2f), gauge: 9, seed: 0, count: 10);
            var ridges = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 2f), magnitudeRange: new(0f, 1f), gauge: 4, seed: 0, count: 10);

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            var structures = JsonSerializer.Deserialize<StructureLibrary>(
                File.ReadAllText("Resources/view/structures.json"), options)!;

            return new(Texture.Create(new(1, 1), Color4.White, new()), rivers, ridges, masks, partitions, structures);
        }
    }
}

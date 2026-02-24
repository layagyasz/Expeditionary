using Cardamom;
using Cardamom.Graphics;
using Expeditionary.View.Textures;
using Expeditionary.View.Textures.Generation;
using OpenTK.Mathematics;

namespace Expeditionary.Runners.Loaders
{
    public class RuntimeTextureLibraryLoader
    {
        private readonly GameResources _resources;

        public RuntimeTextureLibraryLoader(GameResources resources)
        {
            _resources = resources;
        }

        public TextureLibrary Load()
        {
            var partitionTextureGenerator =
                new PartitionTextureGenerator(_resources.GetShader("shader-generate-partition"));
            var partitions =
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 4f), seed: 0, count: 60);

            var maskTextureGenerator = new MaskTextureGenerator(_resources.GetShader("shader-generate-mask"));
            var masks = maskTextureGenerator.Generate(
                frequencyRange: new(4f, 8f), magnitudeRange: new(0f, 1f), seed: 0, count: 16);

            var edgeTextureGenerator = new EdgeTextureGenerator(_resources.GetShader("shader-generate-edge"));
            var rivers = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 2f), gauge: 9, seed: 0, count: 10);
            var ridges = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 2f), magnitudeRange: new(0f, 1f), gauge: 4, seed: 0, count: 10);

            var structureTextureGenerator =
                new StructureTextureGenerator(_resources.GetShader("shader-default-no-tex"));
            var structures = structureTextureGenerator.Generate();

            return new(Texture.Create(new(1, 1), Color4.White, new()), rivers, ridges, masks, partitions, structures);
        }
    }
}

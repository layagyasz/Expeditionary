using Cardamom;
using Expeditionary.Model;
using Expeditionary.Spectra;
using Expeditionary.View.Textures;

namespace Expeditionary
{
    public record class ProgramData(
        GameResources Resources, 
        GameModule Module, 
        SpectrumSensitivity SpectrumSensitivity,
        TextureLibrary TextureLibrary,
        Cardamom.Graphics.TexturePacking.ITextureVolume UnitTextures)
    { }
}

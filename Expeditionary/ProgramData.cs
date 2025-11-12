using Cardamom;
using Cardamom.Audio;
using Expeditionary.Model;
using Expeditionary.Spectra;
using Expeditionary.View.Textures;

namespace Expeditionary
{
    public record class ProgramData(
        GameResources Resources, 
        GameModule Module,
        Playlist Playlist,
        SpectrumSensitivity SpectrumSensitivity,
        TextureLibrary TextureLibrary,
        Cardamom.Graphics.TexturePacking.ITextureVolume UnitTextures)
    { }
}

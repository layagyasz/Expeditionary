using Cardamom;
using Cardamom.Audio;
using Expeditionary.Model;
using Expeditionary.Spectra;
using Expeditionary.View;
using Expeditionary.View.Textures;

namespace Expeditionary
{
    public record class ProgramData(
        ProgramConfig Config,
        GameResources Resources, 
        GameModule Module,
        Playlist Playlist,
        Localization Localization,
        SpectrumSensitivity SpectrumSensitivity,
        TextureLibrary TextureLibrary,
        Cardamom.Graphics.TexturePacking.ITextureVolume UnitTextures)
    { }
}

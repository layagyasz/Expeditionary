namespace Expeditionary
{
    public record class ProgramConfig(
        string Module, 
        string Resources,
        string Playlist, 
        string Localization,
        string SpectrumSensitivity,
        string UnitTextureSettings, 
        bool IsDebug);
}

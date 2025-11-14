namespace Expeditionary
{
    public record class ProgramConfig(
        string Module, 
        string Resources,
        string Playlist, 
        string SpectrumSensitivity,
        string UnitTextureSettings, 
        bool IsDebug);
}

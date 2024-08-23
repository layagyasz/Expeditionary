namespace Expeditionary.Spectra
{
    public class RayleighScatteredSpectrum : ISpectrum
    {
        public ISpectrum Base { get; }

        public RayleighScatteredSpectrum(ISpectrum @base)
        {
            Base = @base;
        }

        public double GetIntensity(float wavelength)
        {
            double w = wavelength * 1e-9f;
            return Base.GetIntensity(wavelength) / (w * w * w * w);
        }
    }
}

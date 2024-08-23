namespace Expeditionary.Spectra
{
    public class BlackbodySpectrum : ISpectrum
    {
        private static readonly double s_Boltzman = 1.3806488e-23f;
        private static readonly float s_C = 299792458;
        private static readonly double s_Planck = 6.62606957e-34f;
        private static readonly float s_Wien = 2897771;

        public float Temperature { get; }

        public BlackbodySpectrum(float temperature)
        {
            Temperature = temperature;
        }

        public double GetIntensity(float wavelength)
        {
            double w = wavelength * 1e-9;
            double left = 2.0 * s_Planck * s_C * s_C / (w * w * w * w * w);
            double right = 1.0 / (Math.Exp(s_Planck * s_C / (w * s_Boltzman * Temperature)) - 1);
            return left * right;
        }

        public float GetPeak()
        {
            return s_Wien / Temperature;
        }
    }
}
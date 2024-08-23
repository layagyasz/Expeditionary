using Cardamom.Mathematics.Color;
using Cardamom.Mathematics;

namespace Expeditionary.Spectra
{
    public class SpectrumSensitivity
    {
        public Interval Range { get; set; }
        public LinearApproximation? X { get; set; }
        public LinearApproximation? Y { get; set; }
        public LinearApproximation? Z { get; set; }

        public ColorCie GetColor(ISpectrum spectrum, float resolution = 1)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            for (float i = Range.Minimum; i <= Range.Maximum; i += resolution)
            {
                double intensity = spectrum.GetIntensity(i);
                x += intensity * X!.GetPoint(i);
                y += intensity * Y!.GetPoint(i);
                z += intensity * Z!.GetPoint(i);
            }
            double s = x + y + z;
            return new((float)(x / s), (float)(y / s));
        }

        public ColorCie GetColor(float wavelength)
        {
            wavelength = Range.Clamp(wavelength);
            double x = X!.GetPoint(wavelength);
            double y = Y!.GetPoint(wavelength);
            double z = Z!.GetPoint(wavelength);
            double s = x + y + z;
            return new((float)(x / s), (float)(y / s));
        }
    }
}

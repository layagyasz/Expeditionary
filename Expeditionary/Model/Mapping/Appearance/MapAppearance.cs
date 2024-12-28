using Cardamom.Mathematics.Color;
using Expeditionary.Spectra;
using Expeditionary.View.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Appearance
{
    public record class MapAppearance
    {
        public int LightTemperature { get; set; }
        public IColoring Liquid { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring GroundCover { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public BarycentricColoringSet Stone { get; set; } = new();
        public BarycentricColoringSet Soil { get; set; } = new();
        public PlantColoringSet Brush { get; set; } = new();
        public PlantColoringSet Foliage { get; set; } = new();

        public TerrainViewParameters Materialize(SpectrumSensitivity sensitivity)
        {
            var wavelength =
                MathHelper.Clamp(
                    new BlackbodySpectrum(LightTemperature).GetPeak(),
                    sensitivity.Range.Minimum + 1,
                    sensitivity.Range.Maximum - 1);
            var baseColor = (Color4)Color4.ToHsv(ColorSystem.Ntsc.Transform(sensitivity.GetColor(wavelength)));
            return new TerrainViewParameters()
            {
                Liquid = Liquid.Get(baseColor),
                GroundCover = GroundCover.Get(baseColor),
                Stone = Stone.Materialize(baseColor),
                Soil = Soil.Materialize(baseColor),
                Brush = Brush.Materialize(baseColor),
                Foliage = Foliage.Materialize(baseColor)
            };
        }
    }
}

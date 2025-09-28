namespace Expeditionary.Model.Sectors
{
    public record struct PlanetKey(int Sector, int System, int Planet)
    {
        public int SectorSeed()
        {
            return HashCode.Combine(Sector);
        }

        public int SystemSeed()
        {
            return HashCode.Combine(Sector, System);
        }

        public int PlanetSeed()
        {
            return HashCode.Combine(Sector, System, Planet);
        }

        public int EnvironmentSeed(int environment)
        {
            return HashCode.Combine(Sector, System, Planet, environment);
        }
    }
}

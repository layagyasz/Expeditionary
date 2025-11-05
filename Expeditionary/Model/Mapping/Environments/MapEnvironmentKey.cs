namespace Expeditionary.Model.Mapping.Environments
{
    public record struct MapEnvironmentKey(int Sector, int System, int Planet, int Environment)
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

        public int EnvironmentSeed()
        {
            return HashCode.Combine(Sector, System, Planet, Environment);
        }
    }
}

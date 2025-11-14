namespace Expeditionary.Model.Mapping.Environments
{
    public record struct MapEnvironmentKey(int Sector, int System, int Planet, int Environment)
    {
        public int SectorSeed()
        {
            return Combine(Sector);
        }

        public int SystemSeed()
        {
            return Combine(Sector, System);
        }

        public int PlanetSeed()
        {
            return Combine(Sector, System, Planet);
        }

        public int EnvironmentSeed()
        {
            return Combine(Sector, System, Planet, Environment);
        }

        private static int Combine(params int[] values)
        {
            int result = 17;
            foreach (int i in values)
            {
                result = 314159 * result + i;
            }
            return result;
        }
    }
}

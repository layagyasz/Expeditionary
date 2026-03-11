namespace Expeditionary.Ai
{
    public static class AiHandlers
    {
        public static IEnumerable<FormationHandler> GetAllComponents(this IAiHandler handler)
        {
            return Enumerable.Concat(handler.Components, handler.Components.SelectMany(child => child.GetAllComponents()));
        }

        public static IEnumerable<DiadHandler> GetAllDiads(this IAiHandler handler)
        {
            return Enumerable.Concat(handler.Diads, handler.Components.SelectMany(diad => diad.GetAllDiads()));
        }
    }
}

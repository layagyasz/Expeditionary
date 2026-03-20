namespace Expeditionary.Model.Matches.Ai
{
    public static class AiHandlers
    {
        public static IEnumerable<FormationHandler> GetAllComponents(this IAiHandler handler)
        {
            return handler.Components.Concat(handler.Components.SelectMany(child => child.GetAllComponents()));
        }

        public static IEnumerable<DiadHandler> GetAllDiads(this IAiHandler handler)
        {
            return handler.Diads.Concat(handler.Components.SelectMany(diad => diad.GetAllDiads()));
        }
    }
}

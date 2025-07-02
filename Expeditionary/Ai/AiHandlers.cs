namespace Expeditionary.Ai
{
    public static class AiHandlers
    {
        public static IEnumerable<DiadHandler> GetAllDiads(this IAiHandler handler)
        {
            return Enumerable.Concat(handler.Diads, handler.Children.SelectMany(x => x.GetAllDiads()));
        }
    }
}

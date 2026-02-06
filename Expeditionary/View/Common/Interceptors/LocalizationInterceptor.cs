using Cardamom.Graphics;

namespace Expeditionary.View.Common.Interceptors
{
    public class LocalizationInterceptor : BaseInterceptor
    {
        public Localization Localization { get; }

        private bool _dirty;

        public LocalizationInterceptor(IRenderable child, Localization localization)
            : base(child)
        {
            Localization = localization;
        }

        public override void Attach()
        {
            Localization.LanguageUpdated += HandleLanguageUpdated;
        }

        public override void Intercept(long delta)
        {
            if (_dirty)
            {
                Refresh();
            }
        }

        private void HandleLanguageUpdated(object? sender, EventArgs e)
        {
            _dirty = true;
        }
    }
}

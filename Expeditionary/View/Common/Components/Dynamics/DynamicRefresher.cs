namespace Expeditionary.View.Common.Components.Dynamics
{
    public class DynamicRefresher
    {
        public long RefreshTime { get; }

        private readonly List<IDynamic> _dynamics = new();

        private long _time;

        public DynamicRefresher(long refreshTime) 
        {
            RefreshTime = refreshTime;
        }

        public void Add(IDynamic dynamic)
        {
            _dynamics.Add(dynamic);
        }

        public void Refresh()
        {
            _dynamics.ForEach(d => d.Refresh());
        }

        public void Update(long delta)
        {
            _time += delta;
            if (_time > RefreshTime)
            {
                _time = 0;
                Refresh();
            }
        }
    }
}

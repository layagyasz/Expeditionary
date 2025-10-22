using Cardamom.Trackers;

namespace Expeditionary.Loader
{
    public class LoaderStatusSegment
    {
        private readonly IntPool _progress = new(0);
        private readonly List<string> _status = new();
        private readonly int _logLength;

        public LoaderStatusSegment(int logLength)
        {
            _logLength = logLength;
        }

        public void AddWork(int amount)
        {
            lock (_progress)
            {
                _progress.ChangeMax(amount);
            }
        }

        public void DoWork()
        {
            lock (_progress)
            {
                _progress.Change(1);
            }
        }

        public float GetPercentDone()
        {
            return _progress.MaxAmount > 0 ? _progress.PercentFull() : 0;
        }

        public void SetStatus(string status)
        {
            lock (_status)
            {
                _status.Insert(0, status);
                if (_status.Count > _logLength)
                {
                    _status.RemoveRange(_logLength, _status.Count - _logLength);
                }
            }
        }
    }
}

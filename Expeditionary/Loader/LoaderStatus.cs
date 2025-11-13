using Cardamom.Trackers;

namespace Expeditionary.Loader
{
    public class LoaderStatus
    {
        public IPool Progress { get; }

        private readonly Dictionary<object, LoaderStatusSegment> _segments = new();
        private readonly List<string> _status = new();
        private readonly int _logLength;

        public LoaderStatus(int logLength)
        {
            _logLength = logLength;
            Progress = 
                new VirtualPool(
                    () => _segments.Sum(x => x.Value.GetPercentDone()),
                    () => _segments.Count);
        }

        public void AddSegment(object segment)
        {
            _segments.Add(segment, new LoaderStatusSegment(_logLength));
        }

        public void AddSegments(params object[] segments)
        {
            foreach (var segment in segments)
            {
                AddSegment(segment);
            }
        }

        public void AddWork(object segment, int amount)
        {
            _segments[segment].AddWork(amount);
        }

        public void DoWork(object segment)
        {
            _segments[segment].DoWork();
        }

        public IEnumerable<string> GetStatus()
        {
            return _status;
        }

        public void SetStatus(object segment, string status)
        {
            lock (_status)
            {
                _status.Insert(0, status);
                if (_status.Count > _logLength)
                {
                    _status.RemoveRange(_logLength, _status.Count - _logLength);
                }
            }
            _segments[segment].SetStatus(status);
        }
    }
}

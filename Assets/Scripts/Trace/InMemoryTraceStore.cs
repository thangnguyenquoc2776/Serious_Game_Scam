using System.Collections.Generic;

namespace SeriousGame.Trace
{
    public class InMemoryTraceStore : ITraceStore
    {
        private readonly List<TraceEvent> _events = new();

        public void Add(TraceEvent e) => _events.Add(e);

        public List<TraceEvent> GetBySession(string sessionId)
        {
            var res = new List<TraceEvent>();
            for (int i = 0; i < _events.Count; i++)
                if (_events[i].sessionId == sessionId) res.Add(_events[i]);
            return res;
        }

        public void ClearSession(string sessionId)
        {
            _events.RemoveAll(x => x.sessionId == sessionId);
        }
    }
}

using System.Collections.Generic;

namespace SeriousGame.Trace
{
    public class InMemoryTraceStore : ITraceStore
    {
        private readonly List<GameTrace> _events = new();

        public void Add(GameTrace trace) => _events.Add(trace);

        public List<GameTrace> GetBySession(string sessionId)
        {
            var res = new List<GameTrace>();
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

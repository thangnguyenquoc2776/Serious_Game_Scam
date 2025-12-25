using System.Collections.Generic;

namespace SeriousGame.Trace
{
    public interface ITraceStore
    {
        void Add(TraceEvent e);
        List<TraceEvent> GetBySession(string sessionId);
        void ClearSession(string sessionId);
    }
}

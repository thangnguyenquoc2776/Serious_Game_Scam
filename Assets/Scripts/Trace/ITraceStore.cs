using System.Collections.Generic;

namespace SeriousGame.Trace
{
    public interface ITraceStore
    {
        void Add(GameTrace trace);
        List<GameTrace> GetBySession(string sessionId);
        void ClearSession(string sessionId);
    }
}

using System;

namespace SeriousGame.Trace
{
    [Serializable]
    public class TraceEvent
    {
        public string sessionId;
        public string episodeId;
        public string beatId;
        public string interactionId;
        public string choiceId;
        public string traceTypeId;

        public long unixMs;
    }
}

using System;
using SeriousGame.State;

namespace SeriousGame.Trace
{
    [Serializable]
    public class TraceEvent
    {
        [Serializable]
        public class ResultData
        {
            public float scoreDelta;
            public bool isCorrect;
        }

        [Serializable]
        public class ContextData
        {
            public string episodeId;
            public string yarnNode;
            public string unityScene;
            public PlayerStateSnapshot stateBefore;
        }

        // Local session linkage for fast filtering.
        public string sessionId;

        public string actor;
        public string verb;
        public string objectId;
        public ResultData result;
        public ContextData context;
        public long unixMs;
    }
}

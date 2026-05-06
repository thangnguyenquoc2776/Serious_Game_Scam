using System;
using SeriousGame.State;
using UnityEngine.SceneManagement;

namespace SeriousGame.Trace
{
    public class TraceService
    {
        private readonly ITraceStore _store;
        private readonly ITraceStore _persistentStore;

        public TraceService(ITraceStore store, ITraceStore persistentStore = null)
        {
            _store = store;
            _persistentStore = persistentStore;
        }

        public void LogEvent(
            string sessionId,
            string actor,
            string verb,
            string objectId,
            TraceEvent.ResultData result,
            TraceEvent.ContextData context)
        {
            if (string.IsNullOrWhiteSpace(verb) || string.IsNullOrWhiteSpace(objectId))
                return;

            var e = new TraceEvent
            {
                sessionId = sessionId ?? "",
                actor = actor ?? "",
                verb = verb,
                objectId = objectId,
                result = result,
                context = context,
                unixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            RecordEventInternal(e);
        }

        public TraceEvent.ContextData BuildContext(
            string episodeId,
            string yarnNode,
            string unityScene,
            PlayerStateSnapshot stateBefore)
        {
            return new TraceEvent.ContextData
            {
                episodeId = episodeId ?? "",
                yarnNode = yarnNode ?? "",
                unityScene = string.IsNullOrWhiteSpace(unityScene) ? SceneManager.GetActiveScene().name : unityScene,
                stateBefore = stateBefore
            };
        }

        private void RecordEventInternal(TraceEvent e)
        {
            _store.Add(e);
            if (_persistentStore != null)
                _persistentStore.Add(e);
        }

        public System.Collections.Generic.List<TraceEvent> GetSession(string sessionId)
            => _store.GetBySession(sessionId);
    }
}

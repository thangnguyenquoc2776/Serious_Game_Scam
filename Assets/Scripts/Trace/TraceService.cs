using System;
using SeriousGame.State;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace SeriousGame.Trace
{
    public class TraceService
    {   
        // 2 different stores: one for in-memory (for quick access during gameplay), and one for persistent storage (e.g. file, database)
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
                actor = actor ?? "", //playerid
                verb = verb,
                objectId = objectId,
                result = result,
                context = context,
                unixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            //store the event in both the in-memory store and the persistent store (if available)
            RecordEventInternal(e);
            Debug.Log($"[TraceService] Logged event: {verb} {objectId} (session: {sessionId}, actor: {actor})");
        }
        // Helper method to build context data for trace events, can be extended with more fields as needed.
        public TraceEvent.ContextData BuildContext(
            string episodeId,
            string yarnNode,
            string unityScene,
            PlayerStateSnapshot stateBefore)
        {
            Debug.Log($"[TraceService] Built context: episodeId={episodeId}, yarnNode={yarnNode}, unityScene={unityScene}");
            return new TraceEvent.ContextData
            {
                episodeId = episodeId ?? "",
                yarnNode = yarnNode ?? "",
                unityScene = string.IsNullOrWhiteSpace(unityScene) ? SceneManager.GetActiveScene().name : unityScene,
                stateBefore = stateBefore
            };
            
        }
        //store the event in both the in-memory store and the persistent store (if available)
        private void RecordEventInternal(TraceEvent e)
        {
            _store.Add(e);
            if (_persistentStore != null)
                _persistentStore.Add(e);
            Debug.Log($"[TraceService] Recorded event: {e.verb} {e.objectId}");
        }

        public System.Collections.Generic.List<TraceEvent> GetSession(string sessionId)
            => _store.GetBySession(sessionId);
    }
}

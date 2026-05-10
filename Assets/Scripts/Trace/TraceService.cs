using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeriousGame.App;
using SeriousGame.Content;
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
        private readonly SessionService _session;
        private readonly PlayerStateService _state;
        private readonly AppConfigSO _config;

        public TraceService(
            ITraceStore store,
            ITraceStore persistentStore,
            SessionService session,
            PlayerStateService state,
            AppConfigSO config)
        {
            _store = store;
            _persistentStore = persistentStore;
            _session = session;
            _state = state;
            _config = config;
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

        public async Task<bool> SendSessionData(bool isCompleted, string chapterId = "")
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            if (_config == null || string.IsNullOrWhiteSpace(_config.firebaseProjectId))
            {
                Debug.LogWarning("[TraceService] Missing Firebase project ID.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_session.AuthToken))
            {
                Debug.LogWarning("[TraceService] Missing auth token.");
                return false;
            }

            var dto = BuildSessionDto(isCompleted, chapterId);
            if (dto == null) return false;

            var cloud = new CloudTraceStore();
            return await cloud.SendSessionData(dto, _config.firebaseProjectId, _session.AuthToken);
        }

        public bool SendSessionDataBlocking(bool isCompleted, string chapterId = "")
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            if (_config == null || string.IsNullOrWhiteSpace(_config.firebaseProjectId))
            {
                Debug.LogWarning("[TraceService] Missing Firebase project ID.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_session.AuthToken))
            {
                Debug.LogWarning("[TraceService] Missing auth token.");
                return false;
            }

            var dto = BuildSessionDto(isCompleted, chapterId);
            if (dto == null) return false;

            var cloud = new CloudTraceStore();
            return cloud.SendSessionDataBlocking(dto, _config.firebaseProjectId, _session.AuthToken);
        }

        private ChapterSessionDTO BuildSessionDto(bool isCompleted, string chapterId)
        {
            if (_session == null) return null;
            var sessionId = _session.CurrentSessionId;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                Debug.LogWarning("[TraceService] SessionId is empty.");
                return null;
            }

            var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var startTime = _session.StartTimeUnixMs > 0 ? _session.StartTimeUnixMs : endTime;
            var totalSeconds = Mathf.Max(0f, (endTime - startTime) / 1000f);

            return new ChapterSessionDTO
            {
                session_id = sessionId,
                player_id = _session.PlayerId ?? string.Empty,
                start_time = startTime,
                end_time = endTime,
                total_playtime_seconds = totalSeconds,
                is_completed = isCompleted,
                final_scores = BuildFinalScores(),
                traces = BuildTraces(sessionId, chapterId)
            };
        }

        private FinalScores BuildFinalScores()
        {
            var scores = new FinalScores();
            if (_state == null) return scores;

            scores.SCORE_HELP_SEEKING = _state.Get(GameStateKeys.ScoreHelpSeeking);
            scores.SCORE_PRESSURE_RESISTANCE = _state.Get(GameStateKeys.ScorePressureResistance);
            scores.SCORE_INFORMATION_VERIFICATION = _state.Get(GameStateKeys.ScoreInformationVerification);
            scores.SCORE_RISK_RECOGNITION = _state.Get(GameStateKeys.ScoreRiskRecognition);
            scores.SCORE_COMMUNITY_WARNING = _state.Get(GameStateKeys.ScoreCommunityWarning);

            return scores;
        }

        private List<GameTrace> BuildTraces(string sessionId, string chapterId)
        {
            var result = new List<GameTrace>();
            var events = GetSession(sessionId);
            if (events == null) return result;

            for (int i = 0; i < events.Count; i++)
            {
                var e = events[i];
                if (e == null) continue;

                var trace = new GameTrace
                {
                    milestone_id = !string.IsNullOrWhiteSpace(chapterId)
                        ? chapterId
                        : (e.context != null ? e.context.episodeId : string.Empty),
                    route_id = e.context != null ? e.context.yarnNode : string.Empty,
                    choice_id = e.objectId ?? string.Empty,
                    trace_id = e.verb ?? string.Empty,
                    action_id = e.verb ?? string.Empty,
                    object_id = e.objectId ?? string.Empty,
                    timestamp = e.unixMs,
                    score_state = BuildScoreState(e.context != null ? e.context.stateBefore : null)
                };

                result.Add(trace);
            }

            return result;
        }

        private Dictionary<string, int> BuildScoreState(PlayerStateSnapshot snapshot)
        {
            var scores = new Dictionary<string, int>();

            scores[GameStateKeys.ScoreHelpSeeking] = GetScore(snapshot, GameStateKeys.ScoreHelpSeeking);
            scores[GameStateKeys.ScorePressureResistance] = GetScore(snapshot, GameStateKeys.ScorePressureResistance);
            scores[GameStateKeys.ScoreInformationVerification] = GetScore(snapshot, GameStateKeys.ScoreInformationVerification);
            scores[GameStateKeys.ScoreRiskRecognition] = GetScore(snapshot, GameStateKeys.ScoreRiskRecognition);
            scores[GameStateKeys.ScoreCommunityWarning] = GetScore(snapshot, GameStateKeys.ScoreCommunityWarning);

            return scores;
        }

        private int GetScore(PlayerStateSnapshot snapshot, string key)
        {
            if (snapshot != null)
                return snapshot.GetValue(key, _state != null ? _state.Get(key) : 0);

            return _state != null ? _state.Get(key) : 0;
        }
    }
}

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
        private string _sentSessionId;
        private string _sendInProgressSessionId;
        private string _completedSessionId;
        private string _completedChapterId;

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

        public GameTrace BuildTrace(
            string sessionId,
            string milestoneId,
            string routeid,
            int choiceId,
            string traceId,
            string objectName,
            // string objectId,
            // string episodeId,
            // string yarnNode,
            PlayerStateSnapshot stateBefore)
        {
            return new GameTrace
            {
                sessionId = sessionId ?? "",
                milestone_id = milestoneId ?? string.Empty,
                route_id = routeid ?? string.Empty,
                choice_id = choiceId.ToString(),
                trace_id = traceId ?? string.Empty,
                object_name = objectName ?? string.Empty,
                // object_id = objectId ?? string.Empty,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                score_state = BuildScoreState(stateBefore)
            };
        }

        public void LogEvent(GameTrace trace)
        {
            if (trace == null) return;
            RecordTraceInternal(trace);
            Debug.Log($"[TraceService] Logged trace event: {trace.trace_id} {trace.route_id} choice {trace.choice_id}");
        }

        //store the trace in both the in-memory store and the persistent store (if available)
        private void RecordTraceInternal(GameTrace trace)
        {
            _store.Add(trace);
            if (_persistentStore != null)
                _persistentStore.Add(trace);
            Debug.Log($"[TraceService] Recorded trace: {trace.trace_id} {trace.route_id} choice {trace.choice_id}");
        }

        public System.Collections.Generic.List<GameTrace> GetSession(string sessionId)
            => _store.GetBySession(sessionId);

        // ham nay se duoc goi khi ket thuc chapter, de gui toan bo trace events trong session do len cloud. Ham su dung async/await de thuc hien viec gui du lieu mot cach non-blocking, giup tranh tinh trang treo game neu viec gui du lieu bi cham hoac gap loi. Tuy nhien, neu muon dam bao viec gui du lieu da hoan thanh truoc khi cho phep player tiep tuc, co the su dung ham SendSessionDataBlocking thay the. Hien tai chi co goi sendsessiondata async, sau nay co the them sendsessiondatablocking neu can.
        public async Task<bool> SendSessionData(bool isCompleted, string chapterId = "")
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            var sessionId = _session.CurrentSessionId;
            if (isCompleted && !string.IsNullOrWhiteSpace(sessionId))
            {
                _completedSessionId = sessionId;
                _completedChapterId = chapterId;
            }

            if (!CanSendSession(sessionId, isCompleted))
                return false;

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
            Debug.Log($"[TraceService] Sending session data for session '{dto.session_id}' with {dto.traces.Count} traces, total playtime {dto.total_playtime_seconds} seconds, is_completed={dto.is_completed}");
            _sendInProgressSessionId = sessionId;
            try
            {
                var ok = await cloud.SendSessionData(dto, _config.firebaseProjectId, _session.AuthToken);
                if (ok)
                {
                    _sentSessionId = sessionId;
                }
                return ok;
            }
            finally
            {
                if (IsSameSession(_sendInProgressSessionId, sessionId))
                    _sendInProgressSessionId = null;
            }
        }

        //ham nay tuong tu nhu SendSessionData, nhung thay vi su dung async/await thi ham se thuc hien gui du lieu mot cach blocking, co the phu hop hon trong mot so tinh huong (vd: khi ket thuc chapter, muon dam bao du lieu da duoc gui len cloud truoc khi cho phep player tiep tuc vao chapter tiep theo)
        public bool SendSessionDataBlocking(bool isCompleted, string chapterId = "")
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            var sessionId = _session.CurrentSessionId;
            if (isCompleted && !string.IsNullOrWhiteSpace(sessionId))
            {
                _completedSessionId = sessionId;
                _completedChapterId = chapterId;
            }

            if (!CanSendSession(sessionId, isCompleted))
                return false;

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
            _sendInProgressSessionId = sessionId;
            try
            {
                var ok = cloud.SendSessionDataBlocking(dto, _config.firebaseProjectId, _session.AuthToken);
                if (ok)
                {
                    _sentSessionId = sessionId;
                }
                return ok;
            }
            finally
            {
                if (IsSameSession(_sendInProgressSessionId, sessionId))
                    _sendInProgressSessionId = null;
            }
        }

        public async Task<bool> SendSessionDataOnQuit()
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            var sessionId = _session.CurrentSessionId;
            if (string.IsNullOrWhiteSpace(sessionId)) return false;

            if (IsSessionCompleted(sessionId))
                return await SendSessionData(true, _completedChapterId ?? string.Empty);

            return await SendSessionData(false);
        }

        public bool SendSessionDataOnQuitBlocking()
        {
            if (_session == null)
            {
                Debug.LogWarning("[TraceService] Missing SessionService.");
                return false;
            }

            var sessionId = _session.CurrentSessionId;
            if (string.IsNullOrWhiteSpace(sessionId)) return false;

            if (IsSessionCompleted(sessionId))
                return SendSessionDataBlocking(true, _completedChapterId ?? string.Empty);

            return SendSessionDataBlocking(false);
        }

        private bool IsSameSession(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right)) return false;
            return string.Equals(left, right, StringComparison.Ordinal);
        }

        private bool IsSessionCompleted(string sessionId)
            => IsSameSession(_completedSessionId, sessionId);

        private bool HasSentSession(string sessionId)
            => IsSameSession(_sentSessionId, sessionId);

        private bool IsSendInProgress(string sessionId)
            => IsSameSession(_sendInProgressSessionId, sessionId);

        private bool CanSendSession(string sessionId, bool isCompleted)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                Debug.LogWarning("[TraceService] SessionId is empty.");
                return false;
            }

            if (IsSendInProgress(sessionId))
            {
                Debug.LogWarning("[TraceService] Session send already in progress.");
                return false;
            }

            if (HasSentSession(sessionId))
            {
                Debug.Log($"[TraceService] Session '{sessionId}' already sent. Skipping duplicate.");
                return false;
            }

            if (!isCompleted && IsSessionCompleted(sessionId))
            {
                Debug.Log($"[TraceService] Session '{sessionId}' already completed. Skipping incomplete send.");
                return false;
            }

            return true;
        }

        //ham nay build ra 1 ChapterSessionDTO tuong ung voi session hien tai, gom thong tin ve session (player id, start time), thong tin ve chapter (chapter id), va list trace events trong session do. Sau do ham se duoc su dung de gui du lieu len cloud.
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
                traces = GetSession(sessionId)
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

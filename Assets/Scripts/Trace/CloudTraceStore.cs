using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SeriousGame.Trace
{
    public class CloudTraceStore
    {
        [Serializable]
        private class TraceBatch
        {
            public string sessionId;
            public string participantId;
            public string chapterId;
            public List<TraceEvent> events = new List<TraceEvent>();
            public long unixMs;
        }

        private readonly string _firebaseUrl;

        public CloudTraceStore(string firebaseUrl)
        {
            _firebaseUrl = firebaseUrl != null ? firebaseUrl.Trim() : string.Empty;
        }

        public IEnumerator PushDataToFirebase(string jsonPayload)
        {
            if (string.IsNullOrWhiteSpace(_firebaseUrl))
            {
                Debug.LogWarning("[CloudTraceStore] Missing Firebase URL.");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                Debug.LogWarning("[CloudTraceStore] Payload is empty.");
                yield break;
            }

            var body = Encoding.UTF8.GetBytes(jsonPayload);
            using (var request = new UnityWebRequest(_firebaseUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"[CloudTraceStore] Sending {body.Length} bytes to {_firebaseUrl}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[CloudTraceStore] Upload success. HTTP {request.responseCode}. Response: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.LogWarning($"[CloudTraceStore] Upload failed. HTTP {request.responseCode}. Error: {request.error}");
                    Debug.LogWarning($"[CloudTraceStore] Response: {request.downloadHandler.text}");
                }
            }
        }

        public static string BuildBatchJson(string sessionId, string participantId, string chapterId, List<TraceEvent> events)
        {
            var batch = new TraceBatch
            {
                sessionId = sessionId ?? string.Empty,
                participantId = participantId ?? string.Empty,
                chapterId = chapterId ?? string.Empty,
                events = events ?? new List<TraceEvent>(),
                unixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            return JsonUtility.ToJson(batch, true);
        }

        public static IEnumerator PushSessionTrace(
            TraceService trace,
            SeriousGame.App.SessionService session,
            string firebaseUrl,
            string chapterId = "")
        {
            if (trace == null || session == null)
            {
                Debug.LogWarning("[CloudTraceStore] Missing TraceService or SessionService.");
                yield break;
            }

            var sessionId = session.CurrentSessionId;
            var participantId = session.ParticipantId;
            var events = trace.GetSession(sessionId);
            var payload = BuildBatchJson(sessionId, participantId, chapterId, events);

            var cloud = new CloudTraceStore(firebaseUrl);
            yield return cloud.PushDataToFirebase(payload);
        }
    }
}

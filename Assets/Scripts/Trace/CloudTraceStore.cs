using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SeriousGame.Trace
{
    public class CloudTraceStore
    {
        public async Task<bool> SendSessionData(ChapterSessionDTO dto, string projectId, string authToken)
        {
            if (dto == null)
            {
                Debug.LogWarning("[CloudTraceStore] DTO is null.");
                return false;
            }

            var url = BuildUrl(projectId, dto.session_id, authToken);
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.LogWarning("[CloudTraceStore] Invalid Firebase URL.");
                return false;
            }

            var json = JsonConvert.SerializeObject(dto);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"[CloudTraceStore] Sending {body.Length} bytes to {url}");
                await SendWebRequestAsync(request);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[CloudTraceStore] Upload success. HTTP {request.responseCode}. Response: {request.downloadHandler.text}");
                    return true;
                }

                Debug.LogWarning($"[CloudTraceStore] Upload failed. HTTP {request.responseCode}. Error: {request.error}");
                Debug.LogWarning($"[CloudTraceStore] Response: {request.downloadHandler.text}");
                return false;
            }
        }

        public bool SendSessionDataBlocking(ChapterSessionDTO dto, string projectId, string authToken, int timeoutMs = 4000)
        {
            if (dto == null)
            {
                Debug.LogWarning("[CloudTraceStore] DTO is null.");
                return false;
            }

            var url = BuildUrl(projectId, dto.session_id, authToken);
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.LogWarning("[CloudTraceStore] Invalid Firebase URL.");
                return false;
            }

            var json = JsonConvert.SerializeObject(dto);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"[CloudTraceStore] Sending {body.Length} bytes to {url} (blocking)");
                var operation = request.SendWebRequest();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                while (!operation.isDone && watch.ElapsedMilliseconds < timeoutMs)
                {
                }

                if (!operation.isDone)
                {
                    request.Abort();
                    Debug.LogWarning("[CloudTraceStore] Upload timed out.");
                    return false;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[CloudTraceStore] Upload success. HTTP {request.responseCode}. Response: {request.downloadHandler.text}");
                    return true;
                }

                Debug.LogWarning($"[CloudTraceStore] Upload failed. HTTP {request.responseCode}. Error: {request.error}");
                Debug.LogWarning($"[CloudTraceStore] Response: {request.downloadHandler.text}");
                return false;
            }
        }

        private static string BuildUrl(string projectId, string sessionId, string authToken)
        {
            if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(authToken))
                return null;

            return $"https://{projectId}.firebaseio.com/sessions/{sessionId}.json?auth={authToken}";
        }

        private static async Task SendWebRequestAsync(UnityWebRequest request)
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
        }
    }
}

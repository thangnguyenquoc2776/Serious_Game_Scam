using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SeriousGame.Players
{
    public class CloudPlayerStore
    {
        public async Task<bool> UpsertPlayer(PlayerRecordDTO dto, string projectId, string authToken)
        {
            if (dto == null)
            {
                Debug.LogWarning("[CloudPlayerStore] DTO is null.");
                return false;
            }

            var url = BuildUrl(projectId, dto.userid, authToken);
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.LogWarning("[CloudPlayerStore] Invalid Firebase URL.");
                return false;
            }

            var json = JsonConvert.SerializeObject(dto);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"[CloudPlayerStore] Sending {body.Length} bytes to {url}");
                await SendWebRequestAsync(request);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[CloudPlayerStore] Upload success. HTTP {request.responseCode}. Response: {request.downloadHandler.text}");
                    return true;
                }

                Debug.LogWarning($"[CloudPlayerStore] Upload failed. HTTP {request.responseCode}. Error: {request.error}");
                Debug.LogWarning($"[CloudPlayerStore] Response: {request.downloadHandler.text}");
                return false;
            }
        }

        private static string BuildUrl(string projectId, string userId, string authToken)
        {
            if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(authToken))
                return null;

            return $"https://{projectId}-default-rtdb.asia-southeast1.firebasedatabase.app/players/{userId}.json?auth={authToken}";
        }

        private static async Task SendWebRequestAsync(UnityWebRequest request)
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
        }
    }

    [Serializable]
    public class PlayerRecordDTO
    {
        public string userid;
        public string email;
        public long created_at;
    }
}

using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SeriousGame.Auth
{
    public class AuthService
    {
        private readonly string _webApiKey;

        public string IdToken { get; private set; }
        public string LocalId { get; private set; }

        public AuthService(string webApiKey)
        {
            _webApiKey = webApiKey != null ? webApiKey.Trim() : string.Empty;
        }

        public async Task<bool> LoginWithEmail(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(_webApiKey))
            {
                Debug.LogWarning("[AuthService] Missing Web API Key.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Debug.LogWarning("[AuthService] Email or password is empty.");
                return false;
            }

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";
            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = JsonConvert.SerializeObject(payload);
            var body = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("[AuthService] Sending login request...");
                await SendWebRequestAsync(request);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[AuthService] Login failed. HTTP {request.responseCode}. Error: {request.error}");
                    Debug.LogWarning($"[AuthService] Response: {request.downloadHandler.text}");
                    return false;
                }

                var responseText = request.downloadHandler.text;
                Debug.Log("[AuthService] Login success.");

                try
                {
                    var response = JsonConvert.DeserializeObject<AuthResponse>(responseText);
                    IdToken = response != null ? response.idToken : null;
                    LocalId = response != null ? response.localId : null;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AuthService] Failed to parse auth response: {ex.Message}");
                    return false;
                }

                return !string.IsNullOrWhiteSpace(IdToken) && !string.IsNullOrWhiteSpace(LocalId);
            }
        }

        private static async Task SendWebRequestAsync(UnityWebRequest request)
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
        }

        [Serializable]
        private class AuthResponse
        {
            public string idToken;
            public string localId;
        }
    }
}

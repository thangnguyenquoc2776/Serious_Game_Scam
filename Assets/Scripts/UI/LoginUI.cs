using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SeriousGame.App;
using SeriousGame.Players;

namespace SeriousGame.UI
{
    public class LoginUI : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Status")]
        [SerializeField] private TMP_Text statusText;

        private bool isLoggingIn;

        public async void OnLoginClicked()
        {
            if (isLoggingIn) return;
            isLoggingIn = true;

            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.Auth == null)
            {
                SetStatus("Auth service not ready.");
                isLoggingIn = false;
                return;
            }

            var email = emailInput != null ? emailInput.text : string.Empty;
            var password = passwordInput != null ? passwordInput.text : string.Empty;
            email = email != null ? email.Trim() : string.Empty;

            SetStatus("Logging in...");
            var ok = await ctx.Auth.LoginWithEmail(email, password);

            if (!ok)
            {
                SetStatus("Login failed. Check credentials.");
                isLoggingIn = false;
                return;
            }

            if (ctx.Session != null)
                ctx.Session.SetAuth(ctx.Auth.LocalId, ctx.Auth.IdToken);

            await TryUpsertPlayerAsync(ctx, email);

            SetStatus("Login success.");
            await LoadMainMenuAsync(ctx);
            isLoggingIn = false;
        }

        private async Task TryUpsertPlayerAsync(AppContext ctx, string email)
        {
            if (ctx == null || ctx.Config == null) return;
            if (string.IsNullOrWhiteSpace(email)) return;

            var userId = ctx.Auth != null ? ctx.Auth.LocalId : null;
            var authToken = ctx.Auth != null ? ctx.Auth.IdToken : null;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(authToken))
            {
                Debug.LogWarning("[LoginUI] Missing auth data for player upload.");
                return;
            }

            var dto = new PlayerRecordDTO
            {
                userid = userId,
                email = email,
                created_at = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var store = new CloudPlayerStore();
            var ok = await store.UpsertPlayer(dto, ctx.Config.firebaseProjectId, authToken);
            if (!ok)
                Debug.LogWarning("[LoginUI] Failed to upsert player record.");
        }

        private async Task LoadMainMenuAsync(AppContext ctx)
        {
            string target = null;
            if (ctx != null && ctx.Config != null)
                target = ctx.Config.mainMenuSceneName;

            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogWarning("[LoginUI] Main menu scene not set.");
                return;
            }

            await Task.Yield();
            SceneManager.LoadScene(target);
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SeriousGame.App;

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

            SetStatus("Login success.");
            await LoadMainMenuAsync(ctx);
            isLoggingIn = false;
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

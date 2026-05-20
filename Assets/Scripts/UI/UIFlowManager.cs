using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SeriousGame.App;

namespace SeriousGame.UI
{
    public class UIFlowManager : MonoBehaviour
    {
        [Header("Scene Overrides (optional)")]
        [SerializeField] private string episodeSceneNameOverride;
        [SerializeField] private string mainMenuSceneNameOverride;

        public void StartGame()
        {
            var ctx = GameBootstrap.Context;
            if (ctx != null && ctx.Session != null)
                ctx.Session.Begin(ctx.Session.PlayerId);

            var target = ResolveEpisodeScene(ctx);
            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogWarning("[UIFlowManager] Episode scene not set.");
                return;
            }

            SceneManager.LoadScene(target);
        }

        public async void QuitToMenu()
        {
            var ctx = GameBootstrap.Context;
            if (ctx != null && ctx.Trace != null && IsEpisodeScene(ctx))
                await ctx.Trace.SendSessionDataOnQuit();

            var target = ResolveMainMenuScene(ctx);
            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogWarning("[UIFlowManager] Main menu scene not set.");
                return;
            }

            SceneManager.LoadScene(target);
        }

        public async void QuitGame()
        {
            var ctx = GameBootstrap.Context;
            if (ctx != null && ctx.Trace != null && IsEpisodeScene(ctx))
                await ctx.Trace.SendSessionDataOnQuit();

            Application.Quit();
        }

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        private string ResolveEpisodeScene(AppContext ctx)
        {
            if (!string.IsNullOrWhiteSpace(episodeSceneNameOverride))
                return episodeSceneNameOverride;
            return ctx != null && ctx.Config != null ? ctx.Config.demoEpisodeSceneName : null;
        }

        private string ResolveMainMenuScene(AppContext ctx)
        {
            if (!string.IsNullOrWhiteSpace(mainMenuSceneNameOverride))
                return mainMenuSceneNameOverride;
            return ctx != null && ctx.Config != null ? ctx.Config.mainMenuSceneName : null;
        }

        private bool IsEpisodeScene(AppContext ctx)
        {
            var episode = ResolveEpisodeScene(ctx);
            if (string.IsNullOrWhiteSpace(episode)) return false;

            var active = SceneManager.GetActiveScene().name;
            if (string.IsNullOrWhiteSpace(active)) return false;

            return string.Equals(active, episode, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}

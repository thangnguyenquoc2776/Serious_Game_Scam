// Assets/Scripts/App/MainMenuStart.cs
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeriousGame.App
{
    public class MainMenuStart : MonoBehaviour
    {
        public void StartDemo()
        {
            var ctx = GameBootstrap.Context;
            var cfg = ctx != null ? ctx.Config : null;

            string target = null;

            if (cfg != null)
            {
                var ep = cfg.defaultEpisode;
                var epScene = ep != null ? ep.GetEntrySceneName() : "";
                target = string.IsNullOrWhiteSpace(epScene) ? cfg.demoEpisodeSceneName : epScene;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogError("[MainMenuStart] Không xác định được scene để start demo (Config thiếu?).");
                return;
            }

            SceneManager.LoadScene(target);
        }
    }
}

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
                target = cfg.demoEpisodeSceneName;

            if (string.IsNullOrWhiteSpace(target))
            {
                Debug.LogError("[MainMenuStart] Không xác định được scene để start demo (Config thiếu?).");
                return;
            }

            SceneManager.LoadScene(target);
        }
    }
}

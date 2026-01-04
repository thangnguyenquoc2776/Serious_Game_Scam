using UnityEngine;
using UnityEngine.SceneManagement;
using SeriousGame.Content;

namespace SeriousGame.App
{
    public class GameBootstrap : MonoBehaviour
    {
        public static AppContext Context { get; private set; }

        [SerializeField] private AppConfigSO config;

        private static GameBootstrap _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (config == null)
            {
                Debug.LogError("[GameBootstrap] Missing AppConfigSO reference.");
                return;
            }

            var ctx = GetComponent<AppContext>();
            if (ctx == null) ctx = gameObject.AddComponent<AppContext>();
            ctx.Init(config);

            Context = ctx;
        }

        public void StartDemo()
        {
            if (config == null) return;

            // Boot scene -> next
            string targetScene;
            if (config.skipMainMenuAndAutoStartEpisode)
            {
                // Prefer Episode entry scene if available
                var ep = config.defaultEpisode;
                var epScene = ep != null ? ep.GetEntrySceneName() : "";
                targetScene = string.IsNullOrWhiteSpace(epScene) ? config.demoEpisodeSceneName : epScene;
            }
            else
            {
                targetScene = config.mainMenuSceneName;
            }

            var active = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrWhiteSpace(targetScene) && active != targetScene)
                SceneManager.LoadScene(targetScene);
        }
    }
}

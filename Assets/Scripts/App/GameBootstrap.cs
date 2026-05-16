using System;
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

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

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

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (config == null) return;
            if (string.IsNullOrWhiteSpace(config.demoEpisodeSceneName)) return;
            if (!string.Equals(scene.name, config.demoEpisodeSceneName, StringComparison.OrdinalIgnoreCase)) return;

            var ctx = Context;
            if (ctx != null && ctx.Session != null)
            {
                if (string.IsNullOrWhiteSpace(ctx.Session.CurrentSessionId))
                    ctx.Session.Begin(ctx.Session.PlayerId);
                else
                    ctx.Session.MarkStartTime();
            }
        }

        private void OnApplicationQuit()
        {
            var ctx = Context;
            if (ctx != null && ctx.Trace != null)
                ctx.Trace.SendSessionDataBlocking(false);
        }

    }
}

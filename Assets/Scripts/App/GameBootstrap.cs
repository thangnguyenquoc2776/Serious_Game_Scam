using UnityEngine;
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

            if (ctx.Session != null && string.IsNullOrWhiteSpace(ctx.Session.CurrentSessionId))
                ctx.Session.Begin();
        }

    }
}

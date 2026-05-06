using UnityEngine;
using Yarn.Unity;
using SeriousGame.App;

namespace SeriousGame.Narrative
{
    public class YarnVariableBridge : MonoBehaviour
    {
        [SerializeField] private AppContext contextOverride;

        private static AppContext _context;

        private void Awake()
        {
            _context = contextOverride != null ? contextOverride : GameBootstrap.Context;
        }

        private void OnEnable()
        {
            _context = contextOverride != null ? contextOverride : GameBootstrap.Context;
        }

        [YarnFunction("get_state")]
        public static int GetState(string key)
        {
            var ctx = _context != null ? _context : GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null) return 0;
            return ctx.PlayerState.Get(key);
        }

        [YarnFunction("get_flag")]
        public static bool GetFlag(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            if (GameStateManager.Instance == null) return false;
            return GameStateManager.Instance.CheckFlag(key);
        }
    }
}

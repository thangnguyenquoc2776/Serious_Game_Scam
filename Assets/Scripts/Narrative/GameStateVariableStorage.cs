using UnityEngine;
using Yarn.Unity;
using SeriousGame.State;

namespace SeriousGame.Narrative
{
    public class GameStateVariableStorage : VariableStorageBehaviour
    {
        private GameStateManager Manager => GameStateManager.Instance;

        public override void Clear()
        {
            // Intentionally no-op to avoid wiping shared state at runtime.
        }

        public override bool TryGetValue(string variableName, out float value)
        {
            value = 0f;
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return false;

            var manager = Manager;
            if (manager == null) return false;

            value = manager.CheckFlag(key) ? 1f : 0f;
            return true;
        }

        public override bool TryGetValue(string variableName, out string value)
        {
            value = null;
            return false;
        }

        public override bool TryGetValue(string variableName, out bool value)
        {
            value = false;
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return false;

            var manager = Manager;
            if (manager == null) return false;

            value = manager.CheckFlag(key);
            return true;
        }

        public override void SetValue(string variableName, float value)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return;

            var manager = Manager;
            if (manager == null) return;

            manager.SetFlag(key, value >= 1f);
        }

        public override void SetValue(string variableName, string value)
        {
            // Only boolean/float flags are supported for GameState keys.
        }

        public override void SetValue(string variableName, bool value)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return;

            var manager = Manager;
            if (manager == null) return;

            manager.SetFlag(key, value);
        }

        private static string Normalize(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName)) return string.Empty;
            return variableName[0] == '$' ? variableName.Substring(1) : variableName;
        }
    }
}

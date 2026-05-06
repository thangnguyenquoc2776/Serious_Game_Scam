using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using SeriousGame.App;
using SeriousGame.State;

namespace SeriousGame.Narrative
{
    public class GameStateVariableStorage : VariableStorageBehaviour
    {
        private PlayerStateService State
        {
            get
            {
                var ctx = GameBootstrap.Context;
                return ctx != null ? ctx.PlayerState : null;
            }
        }

        public override void Clear()
        {
            // Intentionally no-op to avoid wiping shared state at runtime.
        }

        public override bool TryGetValue<T>(string variableName, out T result)
        {
            result = default;

            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return false;

            var state = State;
            if (state == null) return false;

            if (typeof(T) == typeof(bool))
            {
                object value = state.CheckFlag(key);
                result = (T)value;
                return true;
            }

            if (typeof(T) == typeof(float))
            {
                object value = (float)state.Get(key);
                result = (T)value;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                object value = state.CheckFlag(key) ? "true" : "false";
                result = (T)value;
                return true;
            }

            return false;
        }

        public override void SetValue(string variableName, float value)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return;

            var state = State;
            if (state == null) return;

            state.SetFlag(key, value >= 1f);
        }

        public override void SetValue(string variableName, string value)
        {
            // Only boolean/float flags are supported for GameState keys.
        }

        public override void SetValue(string variableName, bool value)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return;

            var state = State;
            if (state == null) return;

            state.SetFlag(key, value);
        }

        public override bool Contains(string variableName)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) return false;
            return State != null;
        }

        public override (Dictionary<string, float> FloatVariables,
            Dictionary<string, string> StringVariables,
            Dictionary<string, bool> BoolVariables) GetAllVariables()
        {
            var floats = new Dictionary<string, float>();
            var strings = new Dictionary<string, string>();
            var bools = new Dictionary<string, bool>();
            var state = State;
            if (state == null) return (floats, strings, bools);

            var snapshot = state.GetFlagsSnapshot();
            if (snapshot == null) return (floats, strings, bools);

            foreach (var kv in snapshot)
            {
                if (!GameStateKeys.IsValid(kv.Key)) continue;
                bools[kv.Key] = kv.Value;
            }

            return (floats, strings, bools);
        }

        public override void SetAllVariables(
            Dictionary<string, float> floatVariables,
            Dictionary<string, string> stringVariables,
            Dictionary<string, bool> boolVariables,
            bool clearExistingVariables)
        {
            var state = State;
            if (state == null) return;

            if (clearExistingVariables)
            {
                // No-op: PlayerState persists globally and should not be wiped here.
            }

            if (boolVariables != null)
            {
                foreach (var kv in boolVariables)
                {
                    var key = Normalize(kv.Key);
                    if (!GameStateKeys.IsValid(key)) continue;
                    state.SetFlag(key, kv.Value);
                }
            }

            if (floatVariables != null)
            {
                foreach (var kv in floatVariables)
                {
                    var key = Normalize(kv.Key);
                    if (!GameStateKeys.IsValid(key)) continue;
                    state.SetFlag(key, kv.Value >= 1f);
                }
            }
        }

        private static string Normalize(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName)) return string.Empty;
            return variableName[0] == '$' ? variableName.Substring(1) : variableName;
        }
    }
}

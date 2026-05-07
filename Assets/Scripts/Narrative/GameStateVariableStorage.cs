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
            if (!GameStateKeys.IsValid(key)) Debug.LogWarning($"[GameStateVariableStorage] Invalid variable name '{variableName}' (normalized to '{key}'). Variable names must be non-empty and can only contain letters, numbers, and underscores.");

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

        public override void SetValue(string variableName, string value)
        {
            // For string values, we only support "true"/"false" for convenience, and store them as boolean flags in the PlayerStateService.
        }
        //set States via Yarn should only accept numeric values
        public override void SetValue(string variableName, float value)
        {
            var key = Normalize(variableName);
            if (!GameStateKeys.IsValid(key)) Debug.LogWarning("Creating new game state keys via Yarn");
            // allow creating new keys via Yarn, but only boolean flags are supported for GameState keys.

            var state = State;
            if (state == null) return;

            state.SetFlag(key, value >= 1f);
        }


        //set Flags via Yarn should only accept boolean values, but we can also interpret "true"/"false" strings for convenience.
        public override void SetValue(string variableName, bool value)
        {
            var key = Normalize(variableName);
             if (!GameStateKeys.IsValid(key)) Debug.LogWarning("Creating new flag keys via Yarn");

            var state = State;
            if (state == null) return;

            state.SetFlag(key, value);
        }

        public override bool Contains(string variableName)
        {
            var key = Normalize(variableName);
            // if (!GameStateKeys.IsValid(key)) return false;
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
                // if (!GameStateKeys.IsValid(kv.Key)) continue;
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
                    // if (!GameStateKeys.IsValid(key)) continue;
                    state.SetFlag(key, kv.Value);
                }
            }

            if (floatVariables != null)
            {
                foreach (var kv in floatVariables)
                {
                    var key = Normalize(kv.Key);
                    // if (!GameStateKeys.IsValid(key)) continue;
                    state.SetFlag(key, kv.Value >= 1f);
                }
            }
        }

        // Normalizes a variable name by removing a leading '$' if present. This allows Yarn scripts to reference variables with or without the '$' prefix for convenience.
        private static string 
        Normalize(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName)) return string.Empty;
            return variableName[0] == '$' ? variableName.Substring(1) : variableName;
        }
    }
}

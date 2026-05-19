using System.Collections.Generic;
using UnityEngine;
using System;

namespace SeriousGame.State
{
    [System.Serializable]
    public class PlayerStateService
    {
        private const int DefaultScoreValue = 50;

        [SerializeField]
        //states includes all numeric states, such as scores and progress indicators. The value is typically clamped between 0 and 100 to represent different levels of achievement or status.

        private readonly Dictionary<string, int> _states = new Dictionary<string, int>();
        [SerializeField]
        private readonly Dictionary<string, bool> _flags = new Dictionary<string, bool>();

        public PlayerStateService()
        {   // Initialize default keys with default values to ensure they exist in the state.
            for (int i = 0; i < GameStateKeys.DefaultKeys.Length; i++)
            {
                var key = GameStateKeys.DefaultKeys[i];
                if (!string.IsNullOrWhiteSpace(key))
                    _states[key] = DefaultScoreValue;
            }
            for (int i = 0; i < GameStateKeys.AllFlagKeys.Length; i++)
            {
                var key = GameStateKeys.AllFlagKeys[i];
                if (!string.IsNullOrWhiteSpace(key))
                    _flags[key] = false;
            }
        }

        public int Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return 0;
            return _states.TryGetValue(key, out var value) ? value : 0;
        }

        public void Set(string key, int value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            _states[key] = Clamp(value);
            Debug.Log($"[PlayerStateService] Set state '{key}' to {value}");
        }

        public void Add(string key, int delta)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var current = Get(key);
            Set(key, current + delta);
        }

        public bool CheckFlag(string key)
        {
            if (!IsFlagKeyAllowed(key)) return false;
            return _flags.TryGetValue(key, out var value) && value;
        }

        public void SetFlag(string key, bool value)
        {
            if (!IsFlagKeyAllowed(key)) return;
            _flags[key] = value;
            Debug.Log($"[PlayerStateService] Set flag '{key}' to {value}");
        }

        public int CountTrue(params string[] keys)
        {
            int count = 0;
            if (keys == null) return 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if (CheckFlag(keys[i])) count++;
            }
            return count;
        }

        public int CountTrueByPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return 0;
            int count = 0;
            foreach (var kv in _flags)
            {
                if (kv.Key.StartsWith(prefix) && kv.Value && IsFlagKeyAllowed(kv.Key))
                    count++;
            }
            return count;
        }

        public IReadOnlyDictionary<string, bool> GetFlagsSnapshot()
        {
            return _flags;
        }

        // Returns a snapshot of the current player state, including both states and flags, which can be used for saving or other purposes.
        public PlayerStateSnapshot GetSnapshot()
        {
            var snapshot = new PlayerStateSnapshot();
            foreach (var kv in _states)
            {
                snapshot.entries.Add(new PlayerStateEntry
                {
                    key = kv.Key,
                    value = kv.Value
                });
            }

            foreach (var kv in _flags)
            {
                snapshot.flags.Add(new PlayerFlagEntry
                {
                    key = kv.Key,
                    value = kv.Value
                });
            }

            return snapshot;
        }

        public void LoadSnapshot(PlayerStateSnapshot snapshot)
        {
            _states.Clear();
            _flags.Clear();
            if (snapshot != null && snapshot.entries != null)
            {
                for (int i = 0; i < snapshot.entries.Count; i++)
                {
                    var entry = snapshot.entries[i];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.key)) continue;
                    _states[entry.key] = Clamp(entry.value);
                }
            }

            if (snapshot != null && snapshot.flags != null)
            {
                for (int i = 0; i < snapshot.flags.Count; i++)
                {
                    var entry = snapshot.flags[i];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.key)) continue;
                    if (!IsFlagKeyAllowed(entry.key)) continue;
                    _flags[entry.key] = entry.value;
                }
            }

            for (int i = 0; i < GameStateKeys.DefaultKeys.Length; i++)
            {
                var key = GameStateKeys.DefaultKeys[i];
                if (!string.IsNullOrWhiteSpace(key) && !_states.ContainsKey(key))
                    _states[key] = DefaultScoreValue;
            }
            for (int i = 0; i < GameStateKeys.AllFlagKeys.Length; i++)
            {
                var key = GameStateKeys.AllFlagKeys[i];
                if (!string.IsNullOrWhiteSpace(key) && !_flags.ContainsKey(key))
                    _flags[key] = false;
            }
        }

        private static int Clamp(int value)
        {
            return Mathf.Clamp(value, 0, 100);
        }

        private static bool IsKeyAllowed(string key)
        {
            return GameStateKeys.IsValid(key);
        }
        private static bool IsFlagKeyAllowed(string key)
        {
            // Nếu bạn muốn Yarn có thể tạo cờ thoải mái mà không cần khai báo trước trong script,
            // bạn có thể chỉ cần đổi hàm này thành: return !string.IsNullOrWhiteSpace(key);
            // Nhưng để quản lý code tốt (như cách bạn đang làm), thì hãy giữ như dưới đây:
            return GameStateKeys.IsValidFlag(key);
        }
    }
}

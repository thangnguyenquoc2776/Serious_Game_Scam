using System.Collections.Generic;
using UnityEngine;

namespace SeriousGame.State
{
    public class PlayerStateService
    {
        private readonly Dictionary<string, int> _states = new Dictionary<string, int>();

        public PlayerStateService()
        {
            for (int i = 0; i < GameStateKeys.DefaultKeys.Length; i++)
            {
                var key = GameStateKeys.DefaultKeys[i];
                if (!string.IsNullOrWhiteSpace(key))
                    _states[key] = 0;
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
        }

        public void Add(string key, int delta)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var current = Get(key);
            Set(key, current + delta);
        }

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
            return snapshot;
        }

        public void LoadSnapshot(PlayerStateSnapshot snapshot)
        {
            _states.Clear();
            if (snapshot != null && snapshot.entries != null)
            {
                for (int i = 0; i < snapshot.entries.Count; i++)
                {
                    var entry = snapshot.entries[i];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.key)) continue;
                    _states[entry.key] = Clamp(entry.value);
                }
            }

            for (int i = 0; i < GameStateKeys.DefaultKeys.Length; i++)
            {
                var key = GameStateKeys.DefaultKeys[i];
                if (!string.IsNullOrWhiteSpace(key) && !_states.ContainsKey(key))
                    _states[key] = 0;
            }
        }

        private static int Clamp(int value)
        {
            return Mathf.Clamp(value, 0, 3);
        }
    }
}

using System;
using System.Collections.Generic;

namespace SeriousGame.State
{
    [Serializable]
    public class PlayerStateEntry
    {
        public string key;
        public int value;
    }

    [Serializable]
    public class PlayerStateSnapshot
    {
        public List<PlayerStateEntry> entries = new List<PlayerStateEntry>();

        public int GetValue(string key, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(key) || entries == null) return defaultValue;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null && entries[i].key == key)
                    return entries[i].value;
            }

            return defaultValue;
        }
    }
}

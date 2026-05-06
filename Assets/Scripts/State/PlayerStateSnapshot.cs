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
    public class PlayerFlagEntry
    {
        public string key;
        public bool value;
    }

    [Serializable]
    public class PlayerStateSnapshot
    {
        public List<PlayerStateEntry> entries = new List<PlayerStateEntry>();
        public List<PlayerFlagEntry> flags = new List<PlayerFlagEntry>();

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

        public bool GetFlag(string key, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(key) || flags == null) return defaultValue;

            for (int i = 0; i < flags.Count; i++)
            {
                if (flags[i] != null && flags[i].key == key)
                    return flags[i].value;
            }

            return defaultValue;
        }
    }
}

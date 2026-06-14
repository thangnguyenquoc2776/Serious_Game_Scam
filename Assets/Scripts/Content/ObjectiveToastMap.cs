using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeriousGame.App
{
    [CreateAssetMenu(menuName = "SeriousGame/Objective Toast Map", fileName = "ObjectiveToastMap")]
    public class ObjectiveToastMap : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string message;
            public string[] objectiveIds;
        }

        [SerializeField] private bool fallbackToObjectiveId = true;
        [SerializeField] private List<Entry> entries = new List<Entry>();

        private Dictionary<string, string> _cache;

        public bool TryGetMessage(string objectiveId, out string message)
        {
            message = null;
            if (string.IsNullOrWhiteSpace(objectiveId)) return false;

            EnsureCache();

            if (_cache.TryGetValue(objectiveId, out message) && !string.IsNullOrWhiteSpace(message))
                return true;

            if (fallbackToObjectiveId)
            {
                message = objectiveId;
                return true;
            }

            return false;
        }

        private void EnsureCache()
        {
            if (_cache != null) return;

            _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                if (entry == null || entry.objectiveIds == null) continue;

                foreach (var id in entry.objectiveIds)
                {
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (_cache.ContainsKey(id)) continue;
                    _cache.Add(id, entry.message);
                }
            }
        }
    }
}

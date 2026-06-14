using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Trace/EvidenceMapping", fileName = "EvidenceMapping")]
    public class EvidenceMappingSO : ScriptableObject
    {
        public EvidenceMappingEntry[] mappings;

        public EvidenceMappingEntry Find(string traceId)
        {
            if (string.IsNullOrWhiteSpace(traceId) || mappings == null) return null;

            foreach (var entry in mappings)
            {
                if (entry != null && entry.traceId == traceId)
                    return entry;
            }

            return null;
        }
    }

    [System.Serializable]
    public class EvidenceMappingEntry
    {
        public string traceId;
        public string displayName;
        [TextArea] public string description;
        public ScoreDelta[] scoreDeltas;
    }

    [System.Serializable]
    public class ScoreDelta
    {
        public string scoreKey;
        public int delta;
    }
}

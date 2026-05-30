using System;
using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Feedback/ScoreFeedbackMapping", fileName = "ScoreFeedbackMapping")]
    public class ScoreFeedbackMappingSO : ScriptableObject
    {
        public const string TotalScoreKey = "TOTAL_SCORE";

        public ScoreFeedbackGroup[] groups;

        public bool TryGetFeedback(string scoreKey, int score, out ScoreFeedbackEntry entry)
        {
            entry = null;
            if (groups == null || string.IsNullOrWhiteSpace(scoreKey)) return false;

            for (int i = 0; i < groups.Length; i++)
            {
                var group = groups[i];
                if (group == null || string.IsNullOrWhiteSpace(group.scoreKey)) continue;
                if (!string.Equals(group.scoreKey, scoreKey, StringComparison.Ordinal)) continue;

                entry = group.Find(score);
                return entry != null;
            }

            return false;
        }

        public bool TryGetTotalFeedback(int score, out ScoreFeedbackEntry entry)
        {
            return TryGetFeedback(TotalScoreKey, score, out entry);
        }
    }

    [Serializable]
    public class ScoreFeedbackGroup
    {
        public string scoreKey;
        public ScoreFeedbackEntry[] entries;

        public ScoreFeedbackEntry Find(int score)
        {
            if (entries == null) return null;
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry != null && entry.Contains(score))
                    return entry;
            }

            return null;
        }
    }

    [Serializable]
    public class ScoreFeedbackEntry
    {
        public int minInclusive;
        public int maxInclusive;
        public string ratingLabel;
        [TextArea] public string feedback;

        public bool Contains(int score)
        {
            return score >= minInclusive && score <= maxInclusive;
        }
    }
}

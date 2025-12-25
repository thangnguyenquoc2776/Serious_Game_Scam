using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Content/Episode", fileName = "Episode")]
    public class EpisodeSO : ScriptableObject
    {
        public string episodeId = "E01";
        public string title = "Chương 1 - Demo";
        [TextArea] public string learningOutcome;

        [Header("Option A: Scenes (recommended)")]
        public SceneSO[] scenes;

        [Header("Option B: Direct Beats (quick MVP)")]
        public BeatSO[] beats;

        public BeatSO[] GetAllBeats()
        {
            if (beats != null && beats.Length > 0) return beats;

            if (scenes == null) return new BeatSO[0];
            // flatten
            var list = new System.Collections.Generic.List<BeatSO>();
            foreach (var sc in scenes)
            {
                if (sc == null || sc.beats == null) continue;
                list.AddRange(sc.beats);
            }
            return list.ToArray();
        }

        public string GetEntrySceneName()
        {
            if (scenes != null && scenes.Length > 0 && scenes[0] != null)
                return scenes[0].unitySceneName;
            return string.Empty;
        }
    }
}

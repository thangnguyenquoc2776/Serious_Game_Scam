using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Runtime
{
    public class BeatInteractable : MonoBehaviour, IInteractable
    {
        [Header("Gameplay")]
        public BeatSO beat;

        private bool _used = false;

        public void Interact()
        {
            if (_used) return;

            if (EpisodeController.Instance == null)
            {
                Debug.LogError("[BeatInteractable] EpisodeController missing.");
                return;
            }

            EpisodeController.Instance.RunBeatFromWorld(beat);
            _used = true; // tránh nói lại hoài
        }
    }
}


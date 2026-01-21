using UnityEngine;
using SeriousGame.Content;
using System.Linq;

namespace SeriousGame.Runtime
{
    public interface IInteractable
    {
        void Interact();
    }
    public class BeatInteractable : MonoBehaviour, IInteractable
    {
        [Header("Các Beat ID object này có thể kích")]
        public string[] supportedBeatIds;// array

        [Header("UI Prompt")]
        public GameObject interactPrompt;

        public void Interact()
        {
            var currentBeat = EpisodeController.Instance.GetCurrentBeat();
            if (currentBeat == null) return;

            // Object này có hỗ trợ beat hiện tại không?
            if (supportedBeatIds.Contains(currentBeat.beatId))
            {
                EpisodeController.Instance.OnWorldInteract(currentBeat.beatId);
            }
            else
            {
                Debug.Log($"[BeatInteractable] Beat {currentBeat.beatId} không hợp với object {name}");
            }
        }

        void Update()
        {
            UpdatePrompt();
        }

        void UpdatePrompt()
        {
            if (interactPrompt == null) return;

            var currentBeat = EpisodeController.Instance.GetCurrentBeat();
            if (currentBeat == null)
            {
                interactPrompt.SetActive(false);
                return;
            }

            bool canInteract = supportedBeatIds.Contains(currentBeat.beatId);
            interactPrompt.SetActive(canInteract);
        }
    }

}

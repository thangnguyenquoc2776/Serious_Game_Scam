using System;
using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Runtime
{
    public class BeatRunner : MonoBehaviour
    {
        public InteractionRouter router;

        public void RunInteraction(InteractionSO interaction, Action onComplete)
        { // chạy các thoại phụ (Outcome)
            if (interaction == null || router == null)
            {
                onComplete?.Invoke();
                return;
            }

            // Chúng ta gọi router nhưng truyền beat = null để router biết đây là thoại phụ (Outcome)
            // Khi người chơi đọc xong hết thoại, onComplete sẽ được gọi để EpisodeController chạy tiếp.
            router.Route(null, interaction, (b, c) => onComplete?.Invoke());
        }
        public void RunBeat(BeatSO beat, Action<BeatSO, ChoiceSO> onChoiceResolved)
        {
            if (beat == null)
            {
                Debug.LogError("[BeatRunner] beat is null.");
                return;
            }

            var interaction = beat.GetPrimaryInteraction();
            if (interaction == null)
            {
                Debug.LogError($"[BeatRunner] Beat {beat.beatId} has no interaction.");
                return;
            }

            if (router == null)
            {
                Debug.LogError("[BeatRunner] router is null.");
                return;
            }

            router.Route(beat, interaction, onChoiceResolved);
        }
    }
}

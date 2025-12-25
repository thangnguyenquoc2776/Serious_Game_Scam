using System;
using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Runtime
{
    public class BeatRunner : MonoBehaviour
    {
        public InteractionRouter router;

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

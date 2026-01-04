using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Content/Choice", fileName = "Choice")]
    public class ChoiceSO : ScriptableObject
    {
        public string choiceId = "C01";
        public string text;
        public string stateToTrigger;

        [Header("Trace (empty => not meaningful => no log)")]
        public string traceTypeId;

        [Header("Outcome (immediate, optional)")]
        [TextArea] public string outcomeText;

        [Header("Flow")]
        // public string nextBeatId; // optional; if empty => go next index
        public BeatSO nextBeat;
        public InteractionSO outcomeInteraction;
    }
}

using System;
using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Phone
{
    public class PhoneUIRoot : MonoBehaviour
    {
        public ChatPanel chatPanel;

        public void ShowChat(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChoiceResolved)
        {
            if (chatPanel == null)
            {
                Debug.LogError("[PhoneUIRoot] chatPanel missing.");
                onChoiceResolved?.Invoke(beat, null);
                return;
            }

            chatPanel.gameObject.SetActive(true);
            chatPanel.Bind(beat, interaction, onChoiceResolved);
        }
    }
}

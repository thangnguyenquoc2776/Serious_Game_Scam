// Assets/Scripts/Runtime/InteractionRouter.cs
using System;
using UnityEngine;
using SeriousGame.Content;
using SeriousGame.Phone;

namespace SeriousGame.Runtime
{
    public class InteractionRouter : MonoBehaviour
    {
        // public PhoneUIRoot phoneUI;
        public DialogueUI dialogueUI;
        public PCUI pcUI;

        public void Route(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChoiceResolved)
        {
            if (interaction == null)
            {
                onChoiceResolved?.Invoke(beat, null);
                return;
            }
 
            switch (interaction.type)
            {
                case InteractionType.Dialogue:
                    Debug.Log("[InteractionRouter] Routing to DialogueUI.");
                    dialogueUI.Show(beat, interaction, onChoiceResolved);
                    break;


                case InteractionType.PC:
                    pcUI.Show(beat, interaction, () =>
                    {
                        onChoiceResolved?.Invoke(beat, null);
                    });
                    break;


                // Các type khác: MVP vẫn render bằng ChatPanel, nhưng cảnh báo để khỏi “quên”
                default:
                    Debug.LogWarning($"[InteractionRouter] Type {interaction.type} chưa có panel riêng. Fallback -> ChatPanel.");
                    // phoneUI.ShowChat(beat, interaction, onChoiceResolved);
                    break;
            }
        }
    }
}

using System;
using UnityEngine;
using TMPro;
using SeriousGame.Content;
using SeriousGame.UI;

namespace SeriousGame.Phone
{
    public class ChatPanel : MonoBehaviour
    {
        public TMP_Text promptText;
        public Transform choicesRoot;
        public ChoiceButton choiceButtonPrefab;

        public void Bind(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChoiceResolved)
        {
            if (promptText != null)
                promptText.text = interaction != null ? interaction.prompt : "";

            // Clear old
            if (choicesRoot != null)
            {
                for (int i = choicesRoot.childCount - 1; i >= 0; i--)
                    Destroy(choicesRoot.GetChild(i).gameObject);
            }

            if (interaction == null || interaction.choices == null || choiceButtonPrefab == null)
            {
                onChoiceResolved?.Invoke(beat, null);
                return;
            }

            foreach (var c in interaction.choices)
            {
                if (c == null) continue;
                var btn = Instantiate(choiceButtonPrefab, choicesRoot);
                btn.SetText(c.text);

                btn.button.onClick.RemoveAllListeners();
                btn.button.onClick.AddListener(() =>
                {
                    onChoiceResolved?.Invoke(beat, c);
                });
            }
        }
    }
}

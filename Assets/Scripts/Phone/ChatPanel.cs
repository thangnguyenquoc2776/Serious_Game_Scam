using System;
using UnityEngine;
using TMPro;
using SeriousGame.Content;
using SeriousGame.UI;
using System.Text; // Thêm cái này để nối chuỗi cho đẹp

namespace SeriousGame.Phone
{
    public class ChatPanel : MonoBehaviour
    {
        public TMP_Text promptText;
        public Transform choicesRoot;
        public ChoiceButton choiceButtonPrefab;

        public void Bind(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChoiceResolved)
        {
            // --- SỬA LỖI PROMPT Ở ĐÂY ---
            if (promptText != null && interaction != null && interaction.dialogueLines != null)
            {
                // Vì đây là Chat, mình sẽ nối các câu thoại lại thành một đoạn tin nhắn
                // Hoặc bạn có thể dùng vòng lặp để tạo ra nhiều tin nhắn riêng biệt sau này
                StringBuilder sb = new StringBuilder();
                foreach (var line in interaction.dialogueLines)
                {
                    sb.AppendLine(line);
                    sb.AppendLine(); // Thêm khoảng cách giữa các tin nhắn
                }
                promptText.text = sb.ToString();
            }
            else
            {
                if (promptText != null) promptText.text = "";
            }

            // --- PHẦN CLEAR VÀ CHOICES GIỮ NGUYÊN ---
            if (choicesRoot != null)
            {
                for (int i = choicesRoot.childCount - 1; i >= 0; i--)
                    Destroy(choicesRoot.GetChild(i).gameObject);
            }

            if (interaction == null || interaction.choices == null || choiceButtonPrefab == null)
            {
                // Nếu không có lựa chọn nào, cho phép kết thúc beat
                // Lưu ý: Có thể bạn muốn chờ người chơi bấm gì đó mới invoke null
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
using UnityEngine;
using UnityEngine.UI;
using SeriousGame.Content;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace SeriousGame.Runtime
{
    public class DialogueUI : MonoBehaviour
    {
        public TextMeshProUGUI promptText;
        public Transform choiceContainer;
        public Button choiceButtonPrefab;
        
        // Các script điều khiển player sẽ bị tắt khi thoại đang mở (movement, nhìn chuột, interact, v.v.)
        public PlayerController scriptsToDisableDuringDialogue;

        private BeatSO _currentBeat; // Add this to store the beat
        private Action<BeatSO, ChoiceSO> _onChoose; // Update signature

        private InteractionSO _currentInteraction;
        private int _currentLineIndex;
        // private Action<ChoiceSO> _onChoose;

        void Update()
        {
            // Bấm chuột trái hoặc Space để sang câu tiếp theo
            if (!gameObject.activeSelf)
                return;

            // Nếu đang hiển thị lựa chọn thì KHÔNG tự advance nữa,
            // để chuột chỉ dùng cho button UI
            if (choiceContainer != null && choiceContainer.gameObject.activeSelf)
                return;

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("[DialogueUI] Advancing dialogue...");
                AdvanceDialogue();
            }
        }


        public void Show(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChooseCallback)
        {
            gameObject.SetActive(true);
            _currentBeat = beat; // Store the beat
            _currentInteraction = interaction;
            _onChoose = onChooseCallback;
            _currentLineIndex = 0;

            choiceContainer.gameObject.SetActive(false);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Tắt các script điều khiển player trong lúc đang thoại
            SetPlayerControlEnabled(false);

            DisplayCurrentLine();
        }

        void DisplayCurrentLine()
        {
            // Lấy câu thoại hiện tại từ mảng dialogueLines chúng ta vừa sửa ở SO
            if (_currentInteraction.dialogueLines != null) promptText.text = _currentInteraction.dialogueLines[_currentLineIndex];
            Debug.Log($"[DialogueUI] Displaying line {_currentLineIndex}: {_currentInteraction.dialogueLines[_currentLineIndex]}");
        }

        void AdvanceDialogue()
        {
            // Nếu vẫn còn câu thoại trong mảng
            if (_currentLineIndex < _currentInteraction.dialogueLines.Length - 1)
            {
                _currentLineIndex++;
                DisplayCurrentLine();
            }
            else
            {
                // Nếu đã hết câu thoại

                // Trường hợp interaction KHÔNG có lựa chọn: tự kết thúc và callback
                if (_currentInteraction.choices == null || _currentInteraction.choices.Length == 0)
                {
                    EndInteractionWithoutChoice();
                }
                else
                {
                    // Nếu có lựa chọn thì hiện các nút lựa chọn ra
                    ShowChoices();
                }
            }
        }

        void ShowChoices()
        {
            if (choiceContainer.gameObject.activeSelf) return; // Tránh tạo trùng

            Debug.Log("[DialogueUI] ShowChoices called");
            choiceContainer.gameObject.SetActive(true);
            foreach (Transform c in choiceContainer) Destroy(c.gameObject);

            if (_currentInteraction.choices == null || _currentInteraction.choices.Length == 0)
            {
                // Phòng hờ: nếu bị gọi nhưng không có choice thì kết thúc luôn
                EndInteractionWithoutChoice();
                return;
            }

            foreach (var choice in _currentInteraction.choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choiceContainer);
                // Dùng TMP_Text nếu prefab của bạn là TextMeshPro, nếu không dùng Text
                var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.text;

                btn.onClick.AddListener(() => Choose(choice));
            }
        }

        void Choose(ChoiceSO choice)
        {
            Debug.Log($"[DialogueUI] Choice clicked: {choice?.text}");
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetPlayerControlEnabled(true);
            _onChoose?.Invoke(_currentBeat, choice);
        }

        void EndInteractionWithoutChoice()
        {
            // Đóng UI và callback với choice = null
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetPlayerControlEnabled(true);
            _onChoose?.Invoke(_currentBeat, null);
        }

        void SetPlayerControlEnabled(bool enabled)
        {
            if (scriptsToDisableDuringDialogue == null) return;

            scriptsToDisableDuringDialogue.enabled = enabled;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using SeriousGame.Content;
using System;
using TMPro;
using System.Collections.Generic;

namespace SeriousGame.Runtime
{
    public class DialogueUI : MonoBehaviour
    {
        public TextMeshProUGUI promptText;
        public Transform choiceContainer;
        public Button choiceButtonPrefab;
        private BeatSO _currentBeat; // Add this to store the beat
        private Action<BeatSO, ChoiceSO> _onChoose; // Update signature

        private InteractionSO _currentInteraction;
        private int _currentLineIndex;
        // private Action<ChoiceSO> _onChoose;

        void Update()
        {
            // Bấm chuột trái hoặc Space để sang câu tiếp theo
            if (gameObject.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            {
                AdvanceDialogue();
            }
        }

        // public void Show(InteractionSO interaction, Action<ChoiceSO> onChooseCallback)
        // {
        //     gameObject.SetActive(true);
        //     _currentInteraction = interaction;
        //     _onChoose = onChooseCallback;
        //     _currentLineIndex = 0;

        //     // Ẩn container chứa nút lựa chọn lúc bắt đầu
        //     choiceContainer.gameObject.SetActive(false);
            
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;

        //     DisplayCurrentLine();
        // }

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

            DisplayCurrentLine();
        }

        void DisplayCurrentLine()
        {
            // Lấy câu thoại hiện tại từ mảng dialogueLines chúng ta vừa sửa ở SO
            promptText.text = _currentInteraction.dialogueLines[_currentLineIndex];
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
                // Nếu đã hết câu thoại, hiện các nút lựa chọn ra
                ShowChoices();
            }
        }

        void ShowChoices()
        {
            if (choiceContainer.gameObject.activeSelf) return; // Tránh tạo trùng

            choiceContainer.gameObject.SetActive(true);
            foreach (Transform c in choiceContainer) Destroy(c.gameObject);

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
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _onChoose?.Invoke(_currentBeat, choice);
        }
    }
}
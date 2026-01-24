using UnityEngine;
using UnityEngine.UI;
using SeriousGame.Content;
using System;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;

namespace SeriousGame.Runtime
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI promptText;
        public Transform choiceContainer;
        public Button choiceButtonPrefab;

        [Header("Lock while dialogue is open (VR/PC)")]
        // Kéo vào đây: Dynamic Move Provider, Snap Turn Provider, Teleport Provider, VRBeatInteractor (world interact), v.v.
        public Behaviour[] behavioursToDisableDuringDialogue;

        [Header("Advance input (VR)")]
        // Kéo action của tay phải vào đây (vd A button, hoặc XRI Right Interaction/Activate)
        public InputActionReference advanceAction;
        public bool allowAdvanceWhileChoicesVisible = false;

        [Header("World Space placement (optional)")]
        public Transform worldAnchor;        // nếu set, UI sẽ xuất hiện tại đây
        public float spawnDistance = 1.2f;   // nếu không có anchor, spawn trước mặt camera
        public Vector3 spawnOffset = new Vector3(0f, -0.15f, 0f);
        public bool faceCamera = true;

        private BeatSO _currentBeat;
        private Action<BeatSO, ChoiceSO> _onChoose;
        private InteractionSO _currentInteraction;
        private int _currentLineIndex;

        private void OnEnable()
        {
            if (advanceAction != null && advanceAction.action != null)
                advanceAction.action.Enable();
        }

        private void OnDisable()
        {
            if (advanceAction != null && advanceAction.action != null)
                advanceAction.action.Disable();
        }

        void Update()
        {
            if (!gameObject.activeSelf) return;

            // Nếu đang hiện choices thì không auto-advance (giống bản PC)
            if (!allowAdvanceWhileChoicesVisible &&
                choiceContainer != null && choiceContainer.gameObject.activeSelf)
                return;

            // VR: bấm nút tay phải để next
            if (advanceAction != null && advanceAction.action != null &&
                advanceAction.action.WasPressedThisFrame())
            {
                AdvanceDialogue();
            }
        }

        public void Show(BeatSO beat, InteractionSO interaction, Action<BeatSO, ChoiceSO> onChooseCallback)
        {
            gameObject.SetActive(true);

            _currentBeat = beat;
            _currentInteraction = interaction;
            _onChoose = onChooseCallback;
            _currentLineIndex = 0;

            if (choiceContainer != null)
                choiceContainer.gameObject.SetActive(false);

            // VR: đặt UI ra world space (nếu cần)
            PlaceDialogueInWorld();

            // Khoá movement + xoay + world interact...
            SetControlsEnabled(false);

            DisplayCurrentLine();
        }

        private void PlaceDialogueInWorld()
        {
            // Nếu ông không muốn tự move UI thì có thể bỏ trống phần này,
            // và đặt canvas sẵn trong scene.
            var cam = Camera.main != null ? Camera.main.transform : null;
            if (cam == null) return;

            Transform t = transform; // hoặc transform.root nếu UI nằm sâu
            if (worldAnchor != null)
            {
                t.position = worldAnchor.position;
                t.rotation = worldAnchor.rotation;
                return;
            }

            t.position = cam.position + cam.forward * spawnDistance + cam.TransformVector(spawnOffset);

            if (faceCamera)
            {
                Vector3 toCam = (cam.position - t.position);
                toCam.y = 0f; // xoay theo ngang cho đỡ chóng mặt
                if (toCam.sqrMagnitude > 0.0001f)
                    t.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
            }
        }

        void DisplayCurrentLine()
        {
            if (_currentInteraction.dialogueLines != null && _currentInteraction.dialogueLines.Length > 0)
                promptText.text = _currentInteraction.dialogueLines[_currentLineIndex];
        }

        void AdvanceDialogue()
        {
            if (_currentInteraction == null || _currentInteraction.dialogueLines == null) return;

            if (_currentLineIndex < _currentInteraction.dialogueLines.Length - 1)
            {
                _currentLineIndex++;
                DisplayCurrentLine();
            }
            else
            {
                if (_currentInteraction.choices == null || _currentInteraction.choices.Length == 0)
                    EndInteractionWithoutChoice();
                else
                    ShowChoices();
            }
        }

        void ShowChoices()
        {
            if (choiceContainer.gameObject.activeSelf) return;

            choiceContainer.gameObject.SetActive(true);
            foreach (Transform c in choiceContainer) Destroy(c.gameObject);

            if (_currentInteraction.choices == null || _currentInteraction.choices.Length == 0)
            {
                EndInteractionWithoutChoice();
                return;
            }

            foreach (var choice in _currentInteraction.choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choiceContainer);
                var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.text;

                btn.onClick.AddListener(() => Choose(choice));
            }
        }

        void Choose(ChoiceSO choice)
        {
            gameObject.SetActive(false);
            SetControlsEnabled(true);
            _onChoose?.Invoke(_currentBeat, choice);
        }

        void EndInteractionWithoutChoice()
        {
            gameObject.SetActive(false);
            SetControlsEnabled(true);
            _onChoose?.Invoke(_currentBeat, null);
        }

        void SetControlsEnabled(bool enabled)
        {
            if (behavioursToDisableDuringDialogue == null) return;

            // enabled = true nghĩa là đóng dialogue -> bật lại controls
            // enabled = false nghĩa là mở dialogue -> tắt controls
            foreach (var b in behavioursToDisableDuringDialogue)
            {
                if (b != null) b.enabled = enabled;
            }
        }
    }
}

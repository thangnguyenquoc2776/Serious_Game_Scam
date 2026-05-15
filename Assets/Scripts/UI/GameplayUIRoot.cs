using System;
using TMPro;
using UnityEngine;
using SeriousGame.App;
using SeriousGame.Runtime;

namespace SeriousGame.UI
{
    public class GameplayUIRoot : MonoBehaviour
    {
        [Serializable]
        private class StaticUIEntry
        {
            public string id;
            public GameObject root;
        }

        [Header("Chat Panels")]
        [SerializeField] private ChatDialoguePanel phonePanel;
        [SerializeField] private ChatDialoguePanel pcPanel;

        [Header("Summary")]
        [SerializeField] private SummaryUIListener summaryUI;

        [Header("Toast")]
        [SerializeField] private OutcomeToastUI toastUI;

        [Header("Hint")]
        [SerializeField] private GameObject hintRoot;
        [SerializeField] private TMP_Text hintText;

        [Header("Optional Player Lock")]
        [SerializeField] private PlayerController player;
        [SerializeField] private bool lockPlayerOnOverlay = true;

        [Header("UI Presenters (Canvas Groups)")]
        [SerializeField] private CanvasGroup normalDialogueUI;
        [SerializeField] private CanvasGroup phoneChatUI;
        [SerializeField] private CanvasGroup PCUI;

        [Header("Static UI (SetActive)")]
        [SerializeField] private StaticUIEntry[] staticUiEntries;
        

        public void HandleSwitchUIRequested(string uitype)
        {
            if (uitype == "phone")
            {
                // Bật Phone, Tắt Normal (Chỉ tắt hiển thị, script Yarn vẫn chạy ngầm)
                ToggleCanvasGroup(phoneChatUI, true);
                ToggleCanvasGroup(normalDialogueUI, false);
                ToggleCanvasGroup(PCUI, false);
            }
            else if (uitype == "normal")
            {
                // Bật Normal, Tắt Phone
                ToggleCanvasGroup(normalDialogueUI, true);
                ToggleCanvasGroup(phoneChatUI, false);
                ToggleCanvasGroup(PCUI, false);
            }
             else if (uitype == "pc")
            {
                // Bật PC, Tắt Normal
                ToggleCanvasGroup(PCUI, true);
                ToggleCanvasGroup(normalDialogueUI, false);
                ToggleCanvasGroup(phoneChatUI, false);
            }
            else
            {
                HandleStaticUI(uitype);
            }
        }

        private void HandleStaticUI(string uitype)
        {
            if (string.IsNullOrWhiteSpace(uitype)) return;

            const string showPrefix = "show:";
            const string hidePrefix = "hide:";

            if (uitype.StartsWith(showPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var id = uitype.Substring(showPrefix.Length).Trim();
                SetStaticUiActive(id, true);
                return;
            }

            if (uitype.StartsWith(hidePrefix, StringComparison.OrdinalIgnoreCase))
            {
                var id = uitype.Substring(hidePrefix.Length).Trim();
                SetStaticUiActive(id, false);
                return;
            }

            // Default: treat the value as an id to show.
            SetStaticUiActive(uitype.Trim(), true);
        }

        private void SetStaticUiActive(string id, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(id) || staticUiEntries == null) return;

            foreach (var entry in staticUiEntries)
            {
                if (entry == null || entry.root == null) continue;
                if (!string.Equals(entry.id, id, StringComparison.OrdinalIgnoreCase)) continue;

                entry.root.SetActive(isActive);
                return;
            }
        }

        private void ToggleCanvasGroup(CanvasGroup cg, bool isVisible)
        {
            if (cg == null) return;
            cg.alpha = isVisible ? 1f : 0f;
            cg.interactable = isVisible;
            cg.blocksRaycasts = isVisible;
        }

        private void OnEnable()
        {
            GameEventBus.OnPhoneChatRequested += HandlePhoneChatRequested;
            GameEventBus.OnPhoneMessageReceived += HandlePhoneMessage;
            GameEventBus.OnPcChatRequested += HandlePcChatRequested;
            GameEventBus.OnSummaryRequested += HandleSummaryRequested;
            GameEventBus.OnToastRequested += HandleToastRequested;
            GameEventBus.OnHintRequested += HandleHintRequested;
            GameEventBus.OnSwitchUIRequested += HandleSwitchUIRequested;
        }

        private void OnDisable()
        {
            GameEventBus.OnPhoneChatRequested -= HandlePhoneChatRequested;
            GameEventBus.OnPhoneMessageReceived -= HandlePhoneMessage;
            GameEventBus.OnPcChatRequested -= HandlePcChatRequested;
            GameEventBus.OnSummaryRequested -= HandleSummaryRequested;
            GameEventBus.OnToastRequested -= HandleToastRequested;
            GameEventBus.OnHintRequested -= HandleHintRequested;
            GameEventBus.OnSwitchUIRequested -= HandleSwitchUIRequested;
        }

        private void Start()
        {
            ToggleCanvasGroup(normalDialogueUI, true);
            ToggleCanvasGroup(phoneChatUI, false);
            ToggleCanvasGroup(PCUI, false);
        }

        private void HandlePhoneChatRequested(string nodeName)
        {
            if (phonePanel == null) return;
            phonePanel.StartChat(nodeName);
            SetPlayerLock(true);
        }

        private void HandlePcChatRequested(string nodeName)
        {
            if (pcPanel == null) return;
            pcPanel.StartChat(nodeName);
            SetPlayerLock(true);
        }

        private void HandlePhoneMessage(string speaker, string message)
        {
            if (phonePanel == null) return;
            phonePanel.AppendMessage(speaker, message);
        }

        private void HandleSummaryRequested()
        {
            if (summaryUI == null) return;
            summaryUI.ShowFromContext();
            SetPlayerLock(true);
        }

        private void HandleToastRequested(string message)
        {
            if (toastUI == null) return;
            toastUI.Show(message);
        }

        private void HandleHintRequested(string message)
        {
            if (hintText != null)
                hintText.text = message ?? string.Empty;

            if (hintRoot != null)
                hintRoot.SetActive(!string.IsNullOrWhiteSpace(message));
        }

        public void ClosePhone()
        {
            if (phonePanel != null)
                phonePanel.Hide();
            SetPlayerLock(false);
        }

        public void ClosePc()
        {
            if (pcPanel != null)
                pcPanel.Hide();
            SetPlayerLock(false);
        }

        public void CloseSummary()
        {
            if (summaryUI != null)
                summaryUI.Hide();
            SetPlayerLock(false);
        }

        private void SetPlayerLock(bool locked)
        {
            if (!lockPlayerOnOverlay || player == null) return;
            player.SetLockState(locked);
        }
    }
}

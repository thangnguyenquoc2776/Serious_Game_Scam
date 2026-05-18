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

        [Header("Static UI Roots")]
        [SerializeField] private GameObject phoneStaticRoot;
        [SerializeField] private GameObject pcStaticRoot;

        [Header("Static UI Images (Children of Image Root)")]
        [SerializeField] private StaticUIEntry[] phoneStaticImages;
        [SerializeField] private StaticUIEntry[] pcStaticImages;
        

        public void HandleSwitchUIRequested(string uitype)
        {
            if (uitype == "phone_chat")
            {
                // Bật Phone, Tắt Normal (Chỉ tắt hiển thị, script Yarn vẫn chạy ngầm)
                HideAllStatic();
                phonePanel?.ClearMessages();
                phonePanel?.Show();
                pcPanel?.Hide();
                ToggleCanvasGroup(phoneChatUI, true);
                ToggleCanvasGroup(normalDialogueUI, false);
                ToggleCanvasGroup(PCUI, false);
            }
            else if (uitype == "phone_chat_keep")
            {
                // Bật Phone và giữ nguyên lịch sử bubble hiện có
                HideAllStatic();
                phonePanel?.Show();
                pcPanel?.Hide();
                ToggleCanvasGroup(phoneChatUI, true);
                ToggleCanvasGroup(normalDialogueUI, false);
                ToggleCanvasGroup(PCUI, false);
            }
            else if (uitype == "normal")
            {
                // Bật Normal, Tắt Phone
                HideAllStatic();
                phonePanel?.Hide();
                pcPanel?.Hide();
                ToggleCanvasGroup(normalDialogueUI, true);
                ToggleCanvasGroup(phoneChatUI, false);
                ToggleCanvasGroup(PCUI, false);
            }
             else if (uitype == "pc")
            {
                // Bật PC, Tắt Normal
                HideAllStatic();
                phonePanel?.Hide();
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
                var args = uitype.Substring(showPrefix.Length).Trim();
                HandleStaticShow(args);
                return;
            }

            if (uitype.StartsWith(hidePrefix, StringComparison.OrdinalIgnoreCase))
            {
                var args = uitype.Substring(hidePrefix.Length).Trim();
                HandleStaticHide(args);
                return;
            }

            // Default: treat the value as an id to show.
            HandleStaticShow(uitype.Trim());
        }

        private void HandleStaticShow(string args)
        {
            var (channel, id) = ParseStaticArgs(args);
            if (string.IsNullOrWhiteSpace(channel))
            {
                // Backward compatibility: no channel -> use phone by default.
                channel = "phone";
                id = args;
            }

            if (string.Equals(channel, "phone", StringComparison.OrdinalIgnoreCase))
            {
                ShowStaticOnRoot(phoneStaticRoot, phoneStaticImages, id);
            }
            else if (string.Equals(channel, "pc", StringComparison.OrdinalIgnoreCase))
            {
                ShowStaticOnRoot(pcStaticRoot, pcStaticImages, id);
            }
        }

        private void HandleStaticHide(string args)
        {
            var (channel, id) = ParseStaticArgs(args);
            if (string.IsNullOrWhiteSpace(channel)) return;

            if (string.Equals(channel, "phone", StringComparison.OrdinalIgnoreCase))
            {
                HideStaticOnRoot(phoneStaticRoot, phoneStaticImages, id);
            }
            else if (string.Equals(channel, "pc", StringComparison.OrdinalIgnoreCase))
            {
                HideStaticOnRoot(pcStaticRoot, pcStaticImages, id);
            }
        }

        private static (string channel, string id) ParseStaticArgs(string args)
        {
            if (string.IsNullOrWhiteSpace(args)) return (string.Empty, string.Empty);

            var parts = args.Split(':');
            if (parts.Length == 1) return (parts[0].Trim(), string.Empty);

            var channel = parts[0].Trim();
            var id = parts[1].Trim();
            return (channel, id);
        }

        private void ShowStaticOnRoot(GameObject root, StaticUIEntry[] entries, string id)
        {
            HideAllDynamic();
            HideStaticOtherRoot(root);
            SetRootVisible(root, true);
            SetStaticImagesActive(entries, id);
        }

        private void HideStaticOnRoot(GameObject root, StaticUIEntry[] entries, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                SetStaticImagesActive(entries, id, false);
                return;
            }

            SetStaticImagesActive(entries, null);
            SetRootVisible(root, false);
        }

        private void HideAllDynamic()
        {
            ToggleCanvasGroup(normalDialogueUI, false);
            ToggleCanvasGroup(phoneChatUI, false);
            ToggleCanvasGroup(PCUI, false);
        }

        private void HideAllStatic()
        {
            SetStaticImagesActive(phoneStaticImages, null);
            SetStaticImagesActive(pcStaticImages, null);
            SetRootVisible(phoneStaticRoot, false);
            SetRootVisible(pcStaticRoot, false);
        }

        private void HideStaticOtherRoot(GameObject activeRoot)
        {
            if (activeRoot == phoneStaticRoot)
            {
                SetStaticImagesActive(pcStaticImages, null);
                SetRootVisible(pcStaticRoot, false);
            }
            else if (activeRoot == pcStaticRoot)
            {
                SetStaticImagesActive(phoneStaticImages, null);
                SetRootVisible(phoneStaticRoot, false);
            }
        }

        private static void SetStaticImagesActive(StaticUIEntry[] entries, string id, bool? forceState = null)
        {
            if (entries == null) return;

            foreach (var entry in entries)
            {
                if (entry == null || entry.root == null) continue;

                var isMatch = !string.IsNullOrWhiteSpace(id)
                    && string.Equals(entry.id, id, StringComparison.OrdinalIgnoreCase);

                var isActive = forceState ?? isMatch;
                entry.root.SetActive(isActive);
            }
        }

        private static void SetRootVisible(GameObject root, bool isVisible)
        {
            if (root == null) return;

            var canvasGroup = root.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isVisible ? 1f : 0f;
                canvasGroup.interactable = isVisible;
                canvasGroup.blocksRaycasts = isVisible;
                return;
            }

            root.SetActive(isVisible);
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
            ToggleCanvasGroup(PCUI, false);
            SetRootVisible(phoneStaticRoot, false);
            SetRootVisible(pcStaticRoot, false);
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
            SetPlayerHardLock(true);
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
            SetPlayerHardLock(false);
        }

        private void SetPlayerLock(bool locked)
        {
            if (!lockPlayerOnOverlay)
            {
                Debug.Log("[GameplayUIRoot] lockPlayerOnOverlay is false; skipping player lock.");
                return;
            }

            var resolvedPlayer = ResolvePlayer();
            if (resolvedPlayer == null) return;

            Debug.Log($"[GameplayUIRoot] Setting player lock = {locked}");
            resolvedPlayer.SetLockState(locked);
        }

        private void SetPlayerHardLock(bool locked)
        {
            if (!lockPlayerOnOverlay)
            {
                Debug.Log("[GameplayUIRoot] lockPlayerOnOverlay is false; skipping player hard lock.");
                return;
            }

            var resolvedPlayer = ResolvePlayer();
            if (resolvedPlayer == null) return;

            Debug.Log($"[GameplayUIRoot] Setting player hard lock = {locked}");
            resolvedPlayer.SetHardLock(locked);
        }

        private PlayerController ResolvePlayer()
        {
            if (player != null) return player;

#if UNITY_2023_1_OR_NEWER || UNITY_2022_2_OR_NEWER
            player = FindAnyObjectByType<PlayerController>();
#else
            player = FindObjectOfType<PlayerController>();
#endif

            if (player == null)
                Debug.LogWarning("[GameplayUIRoot] PlayerController is null; cannot change lock state.");

            return player;
        }
    }
}

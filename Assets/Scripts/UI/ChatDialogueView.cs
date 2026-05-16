using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace SeriousGame.UI
{
    public class ChatDialogueView : DialoguePresenterBase
    {   

        [Header("References")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [Header("Messages")]
        [SerializeField] private Transform contentRoot;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject leftBubblePrefab;
        [SerializeField] private GameObject rightBubblePrefab;
        [SerializeField] private bool includeCharacterName = true;
        [SerializeField] private string playerName = "Minh";

        [Header("Flow")]
        [SerializeField] private bool autoAdvance = true;
        [SerializeField] private float autoAdvanceDelay = 0.2f;
        [SerializeField] private bool captureLines = false;
        [SerializeField] private bool autoScrollToBottom = true;

        private Coroutine scrollRoutine;

        public override YarnTask OnDialogueStartedAsync()
        {
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (!captureLines)
                return;

            var speaker = line.CharacterName ?? string.Empty;
            var text = line.TextWithoutCharacterName.Text;
            AppendMessage(speaker, text);

            if (autoAdvance)
            {
                await YarnTask.Delay((int)(autoAdvanceDelay * 1000f), token.NextContentToken).SuppressCancellationThrow();
            }
            else
            {
                await YarnTask.WaitUntilCanceled(token.NextContentToken).SuppressCancellationThrow();
            }
        }

        public void AppendMessage(string speaker, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            if (contentRoot == null)
            {
                Debug.LogWarning("[ChatDialogueView] Missing contentRoot.");
                return;
            }

            var prefab = ResolveBubblePrefab(speaker);
            if (prefab == null)
            {
                Debug.LogWarning("[ChatDialogueView] Missing bubble prefab.");
                return;
            }

            var bubble = Instantiate(prefab, contentRoot);
            ApplyBubbleText(bubble, speaker, message);

            if (autoScrollToBottom)
                ScrollToBottom();
        }

        public void ClearMessages()
        {
            if (contentRoot == null) return;
            for (int i = contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }
        }

        private GameObject ResolveBubblePrefab(string speaker)
        {
            if (!string.IsNullOrWhiteSpace(playerName)
                && !string.IsNullOrWhiteSpace(speaker)
                && rightBubblePrefab != null
                && string.Equals(playerName, speaker, StringComparison.OrdinalIgnoreCase))
            {
                return rightBubblePrefab;
            }

            return leftBubblePrefab != null ? leftBubblePrefab : rightBubblePrefab;
        }

        private void ScrollToBottom()
        {
            if (scrollRect == null) return;

            if (scrollRoutine != null)
                StopCoroutine(scrollRoutine);

            scrollRoutine = StartCoroutine(ScrollToBottomNextFrame());
        }

        private IEnumerator ScrollToBottomNextFrame()
        {
            yield return new WaitForEndOfFrame();

            if (scrollRect == null) yield break;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }

        public void SetCaptureLines(bool isCapturing)
        {
            captureLines = isCapturing;
        }

        private void ApplyBubbleText(GameObject bubble, string speaker, string message)
        {
            if (bubble == null) return;

            var tmps = bubble.GetComponentsInChildren<TMP_Text>(true);
            TMP_Text nameText = null;
            TMP_Text bodyText = null;

            for (int i = 0; i < tmps.Length; i++)
            {
                var tmp = tmps[i];
                if (tmp == null) continue;

                var name = tmp.gameObject.name;
                if (nameText == null && name.IndexOf("Character", StringComparison.OrdinalIgnoreCase) >= 0)
                    nameText = tmp;
                else if (nameText == null && name.IndexOf("Name", StringComparison.OrdinalIgnoreCase) >= 0)
                    nameText = tmp;

                if (bodyText == null && string.Equals(name, "Text", StringComparison.OrdinalIgnoreCase))
                    bodyText = tmp;
            }

            if (nameText != null)
                nameText.text = includeCharacterName ? speaker : string.Empty;

            if (bodyText != null)
            {
                bodyText.text = message;
                return;
            }

            var fallback = bubble.GetComponentInChildren<TMP_Text>();
            if (fallback != null)
            {
                if (includeCharacterName && !string.IsNullOrWhiteSpace(speaker))
                    fallback.text = speaker + ": " + message;
                else
                    fallback.text = message;
            }
        }

        public void AdvanceDialogue()
      {
        if (dialogueRunner != null)
            {
                // Lệnh này sẽ phát lệnh Cancel đến cái `token.NextContentToken` đang treo ở trên
                dialogueRunner.RequestNextLine();
            }
            else
            {
                Debug.LogWarning("[ChatDialogueView] Ê, quên kéo DialogueRunner vào Inspector kìa!");
            }
        }
    }
}

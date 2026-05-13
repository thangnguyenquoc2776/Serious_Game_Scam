using System;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private string playerName = "Bạn";

        [Header("Options")]
        [SerializeField] private Transform optionsRoot;
        [SerializeField] private GameObject optionButtonPrefab;

        [Header("Flow")]
        [SerializeField] private bool autoAdvance = true;
        [SerializeField] private float autoAdvanceDelay = 0.2f;

        private readonly List<GameObject> optionButtons = new List<GameObject>();

        public override YarnTask OnDialogueStartedAsync()
        {
            ClearOptions();
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            ClearOptions();
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
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

        public override async YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
        {
            if (dialogueOptions == null || dialogueOptions.Length == 0)
                return null; // <-- Changed here

            if (optionsRoot == null || optionButtonPrefab == null)
            {
                Debug.LogWarning("[ChatDialogueView] Missing optionsRoot or optionButtonPrefab.");
                return null; // <-- Changed here
            }

            ClearOptions();

            var tcs = new YarnTaskCompletionSource<DialogueOption?>();
            CancellationTokenRegistration registration = cancellationToken.NextContentToken.Register(() =>
            {
                tcs.TrySetResult(null);
            });

            foreach (var option in dialogueOptions)
            {
                var buttonObject = Instantiate(optionButtonPrefab, optionsRoot);
                optionButtons.Add(buttonObject);

                var button = buttonObject.GetComponent<Button>();
                var label = buttonObject.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = option.Line.TextWithoutCharacterName.Text;

                if (button != null)
                {
                    button.interactable = option.IsAvailable;
                    button.onClick.AddListener(() =>
                    {
                        if (!option.IsAvailable) return;
                        tcs.TrySetResult(option);
                    });
                }
            }

            var selected = await tcs.Task;
            registration.Dispose();
            ClearOptions();
            return selected;
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
            var text = bubble.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                if (includeCharacterName && !string.IsNullOrWhiteSpace(speaker))
                    text.text = speaker + ": " + message;
                else
                    text.text = message;
            }

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

        private void ClearOptions()
        {
            for (int i = 0; i < optionButtons.Count; i++)
            {
                if (optionButtons[i] != null)
                    Destroy(optionButtons[i]);
            }
            optionButtons.Clear();
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0f;
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

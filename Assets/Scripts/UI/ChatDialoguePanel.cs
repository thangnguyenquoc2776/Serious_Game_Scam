using UnityEngine;
using Yarn.Unity;

namespace SeriousGame.UI
{
    public class ChatDialoguePanel : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject root;

        [Header("Yarn")]
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private ChatDialogueView view;

        [Header("Behavior")]
        [SerializeField] private bool hideOnComplete = true;
        [SerializeField] private bool clearOnStart = true;
        [SerializeField] private bool hideWithCanvasGroup = true;

        private CanvasGroup rootCanvasGroup;

        private void Awake()
        {
            if (root == null)
                root = gameObject;

            if (root != null && !root.activeSelf)
                root.SetActive(true);

            rootCanvasGroup = root != null ? root.GetComponent<CanvasGroup>() : null;
            if (hideWithCanvasGroup && root != null && rootCanvasGroup == null)
                rootCanvasGroup = root.AddComponent<CanvasGroup>();

            view?.SetCaptureLines(false);

            if (runner != null)
                runner.onDialogueComplete.AddListener(HandleDialogueComplete);
        }

        private void OnDestroy()
        {
            if (runner != null)
                runner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        }

        public void StartChat(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName)) return;

            if (root != null)
                SetRootVisible(true);

            view?.SetCaptureLines(true);

            if (clearOnStart)
                view?.ClearMessages();

            if (runner == null)
            {
                Debug.LogWarning("[ChatDialoguePanel] Missing DialogueRunner.");
                return;
            }

            if (runner.IsDialogueRunning)
                runner.Stop().Forget();

            runner.StartDialogue(nodeName).Forget();
        }

        public void Show()
        {
            if (root != null)
                SetRootVisible(true);

            view?.SetCaptureLines(true);
        }

        public void AppendMessage(string speaker, string message)
        {
            if (root != null)
                SetRootVisible(true);

            view?.AppendMessage(speaker, message);
        }

        public void ClearMessages()
        {
            view?.ClearMessages();
        }

        public void Hide()
        {
            SetRootVisible(false);

            view?.SetCaptureLines(false);
        }

        private void SetRootVisible(bool isVisible)
        {
            if (root == null) return;

            if (hideWithCanvasGroup && rootCanvasGroup != null)
            {
                rootCanvasGroup.alpha = isVisible ? 1f : 0f;
                rootCanvasGroup.interactable = isVisible;
                rootCanvasGroup.blocksRaycasts = isVisible;
                return;
            }

            root.SetActive(isVisible);
        }

        private void HandleDialogueComplete()
        {
            if (hideOnComplete)
                Hide();
        }
    }
}

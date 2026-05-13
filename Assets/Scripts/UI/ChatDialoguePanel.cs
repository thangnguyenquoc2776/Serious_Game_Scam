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

        private void Awake()
        {
            if (root == null)
                root = gameObject;

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
                root.SetActive(true);

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

        public void AppendMessage(string speaker, string message)
        {
            if (root != null)
                root.SetActive(true);

            view?.AppendMessage(speaker, message);
        }

        public void ClearMessages()
        {
            view?.ClearMessages();
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }

        private void HandleDialogueComplete()
        {
            if (hideOnComplete)
                Hide();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SeriousGame.App;

namespace SeriousGame.Phone
{
    public class PhoneUIListener : MonoBehaviour
    {
        [Header("UI Root")]
        [SerializeField] private GameObject phoneRoot;

        [Header("Message List")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private GameObject messageBubblePrefab;

        private void OnEnable()
        {
            GameEventBus.OnPhoneMessageReceived += HandlePhoneMessage;
        }

        private void OnDisable()
        {
            GameEventBus.OnPhoneMessageReceived -= HandlePhoneMessage;
        }

        private void HandlePhoneMessage(string characterName, string message)
        {
            if (contentRoot == null || messageBubblePrefab == null)
            {
                Debug.LogWarning("[PhoneUIListener] Missing contentRoot or messageBubblePrefab.");
                return;
            }

            if (phoneRoot != null)
                phoneRoot.SetActive(true);

            var bubble = Instantiate(messageBubblePrefab, contentRoot);
            var text = bubble.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                if (!string.IsNullOrWhiteSpace(characterName))
                    text.text = $"{characterName}: {message}";
                else
                    text.text = message;
            }

            // Force layout update before scrolling.
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}

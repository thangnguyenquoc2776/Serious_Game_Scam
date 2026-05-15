// Assets/Scripts/UI/OutcomeToastUI.cs
using UnityEngine;

namespace SeriousGame.UI
{
    public class OutcomeToastUI : MonoBehaviour
    {
        [SerializeField] private float showSeconds = 10f;

        private ToastNotification _toastNotification;

        private void Awake()
        {
            _toastNotification = FindFirstObjectByType<ToastNotification>();
        }

        public void Show(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (_toastNotification == null)
                _toastNotification = FindFirstObjectByType<ToastNotification>();

            if (_toastNotification == null || _toastNotification._messagePrefab == null)
            {
                Debug.LogWarning("[OutcomeToastUI] ToastNotification not found or missing prefab.");
                return;
            }

            ToastNotification.Show(message, showSeconds);
        }

        public void Hide()
        {
            if (_toastNotification != null)
                ToastNotification.Hide();
        }
    }
}

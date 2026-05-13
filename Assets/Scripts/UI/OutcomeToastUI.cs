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
            _toastNotification = FindObjectOfType<ToastNotification>(true);
        }

        public void Show(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (_toastNotification == null)
                _toastNotification = FindObjectOfType<ToastNotification>(true);

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

using UnityEngine;
using SeriousGame.App;

namespace SeriousGame.UI
{
    public class ToastUIListener : MonoBehaviour
    {
        [SerializeField] private OutcomeToastUI toastUI;

        private void OnEnable()
        {
            GameEventBus.OnToastRequested += HandleToastRequested;
        }

        private void OnDisable()
        {
            GameEventBus.OnToastRequested -= HandleToastRequested;
        }

        private void HandleToastRequested(string message)
        {
            if (toastUI == null)
            {
                Debug.LogWarning("[ToastUIListener] OutcomeToastUI not assigned.");
                return;
            }

            if (string.IsNullOrWhiteSpace(message)) return;
            toastUI.Show(message);
        }
    }
}

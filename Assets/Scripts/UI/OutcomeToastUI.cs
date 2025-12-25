// Assets/Scripts/UI/OutcomeToastUI.cs
using System.Collections;
using TMPro;
using UnityEngine;

namespace SeriousGame.UI
{
    public class OutcomeToastUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float showSeconds = 2f;

        private Coroutine _co;

        private void Awake()
        {
            Hide();
        }

        public void Show(string message)
        {
            if (text == null) return;

            text.text = message ?? "";
            gameObject.SetActive(true);

            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(AutoHide());
        }

        public void Hide()
        {
            if (_co != null) StopCoroutine(_co);
            _co = null;
            gameObject.SetActive(false);
        }

        private IEnumerator AutoHide()
        {
            yield return new WaitForSeconds(showSeconds);
            Hide();
        }
    }
}

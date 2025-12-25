using UnityEngine;
using TMPro;

namespace SeriousGame.UI
{
    public class SummaryUI : MonoBehaviour
    {
        public GameObject root;
        public TMP_Text summaryText;

        public void Show(string text)
        {
            if (root != null) root.SetActive(true);
            if (summaryText != null) summaryText.text = text;
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SeriousGame.UI
{
    public class ChoiceButton : MonoBehaviour
    {
        public Button button;
        public TMP_Text label;

        public void SetText(string text)
        {
            if (label != null) label.text = text;
        }
    }
}

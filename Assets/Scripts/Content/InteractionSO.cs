using UnityEngine;

namespace SeriousGame.Content
{
    public enum InteractionType
    {
        Chat,
        Browser,
        BankTransfer,
        PhoneCall,
        Dialogue
    }

    [CreateAssetMenu(menuName = "SeriousGame/Content/Interaction", fileName = "Interaction")]
    public class InteractionSO : ScriptableObject
    {
        public string interactionId = "I01";
        public InteractionType type = InteractionType.Chat;

        // [TextArea] public string prompt;
        [Header("Nội dung thoại (Nhập theo thứ tự)")]
        [TextArea(3, 10)]
        public string[] dialogueLines;
        public ChoiceSO[] choices;

    }
}

using UnityEngine;

namespace SeriousGame.Content
{
    public enum InteractionType
    {
        Chat,
        Browser,
        BankTransfer,
        PhoneCall
    }

    [CreateAssetMenu(menuName = "SeriousGame/Content/Interaction", fileName = "Interaction")]
    public class InteractionSO : ScriptableObject
    {
        public string interactionId = "I01";
        public InteractionType type = InteractionType.Chat;

        [TextArea] public string prompt;
        public ChoiceSO[] choices;

        // optional metadata
        // public string url;
        // public string phoneNumber;
        // public int moneyAmount;
    }
}

using System;

namespace SeriousGame.App
{
    public static class GameEventBus
    {
        // UI requests
        public static event Action<string> OnPhoneChatRequested;
        public static event Action<string, string> OnPhoneMessageReceived;
        public static event Action<string> OnDialogueRequested;
        public static event Action<string> OnToastRequested;
        public static event Action OnSummaryRequested;
        public static event Action<string> OnHintRequested;

        public static void RaisePhoneChatRequested(string interactionId)
        {
            OnPhoneChatRequested?.Invoke(interactionId);
        }

        public static void RaisePhoneMessageReceived(string characterName, string message)
        {
            OnPhoneMessageReceived?.Invoke(characterName, message);
        }

        public static void RaiseDialogueRequested(string nodeName)
        {
            OnDialogueRequested?.Invoke(nodeName);
        }

        public static void RaiseToastRequested(string message)
        {
            OnToastRequested?.Invoke(message);
        }

        public static void RaiseSummaryRequested()
        {
            OnSummaryRequested?.Invoke();
        }

        public static void RaiseHintRequested(string message)
        {
            OnHintRequested?.Invoke(message);
        }
    }
}

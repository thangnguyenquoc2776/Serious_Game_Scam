namespace SeriousGame.State
{
    public static class GameStateKeys
    {
        public const string Money = "STATE_MONEY";
        public const string Suspicion = "STATE_SUSPICION";
        public const string Confidence = "STATE_CONFIDENCE";
        public const string Support = "STATE_SUPPORT";
        public const string Trauma = "STATE_TRAUMA";
        public const string Trust = "STATE_TRUST";

        public static readonly string[] AllKeys =
        {
            Money,
            Suspicion,
            Confidence,
            Support,
            Trauma,
            Trust
        };

        public static readonly string[] DefaultKeys = AllKeys;

        public static bool IsValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            for (int i = 0; i < AllKeys.Length; i++)
                if (AllKeys[i] == key) return true;
            return false;
        }
    }
}

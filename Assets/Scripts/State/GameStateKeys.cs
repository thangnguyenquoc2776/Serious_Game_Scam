namespace SeriousGame.State
{   
    public static class GameStateKeys
    {
        public const string ScoreHelpSeeking = "SCORE_HELP_SEEKING";
        public const string ScorePressureResistance = "SCORE_PRESSURE_RESISTANCE";
        public const string ScoreInformationVerification = "SCORE_INFORMATION_VERIFICATION";
        public const string ScoreRiskRecognition = "SCORE_RISK_RECOGNITION";
        public const string ScoreCommunityWarning = "SCORE_COMMUNITY_WARNING";

        public static readonly string[] AllKeys =
        {
            ScoreHelpSeeking,
            ScorePressureResistance,
            ScoreInformationVerification,
            ScoreRiskRecognition,
            ScoreCommunityWarning
        };

        public static readonly string[] DefaultKeys = AllKeys;


        public const string QR_scan = "QR_SCAN";
        public const string Make_friend = "MAKE_FRIEND";

        public static readonly string[] AllFlagKeys =
        {
            QR_scan,
            Make_friend
        };
        public static bool IsValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            for (int i = 0; i < AllKeys.Length; i++)
                if (AllKeys[i] == key) return true;
            return false;
        }

        public static bool IsValidFlag(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            for (int i = 0; i < AllFlagKeys.Length; i++)
                if (AllFlagKeys[i] == key) return true;
            return false;
        }
    }
}

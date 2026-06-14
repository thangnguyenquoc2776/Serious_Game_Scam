using TMPro;
using UnityEngine;
using SeriousGame.App;
using SeriousGame.State;
using SeriousGame.Content;

namespace SeriousGame.UI
{
    public class SummaryUIListener : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text helpSeekingText;
        [SerializeField] private TMP_Text pressureResistanceText;
        [SerializeField] private TMP_Text informationVerificationText;
        [SerializeField] private TMP_Text riskRecognitionText;
        [SerializeField] private TMP_Text communityWarningText;

        [Header("Feedback UI (optional)")]
        [SerializeField] private TMP_Text helpSeekingFeedbackText;
        [SerializeField] private TMP_Text pressureResistanceFeedbackText;
        [SerializeField] private TMP_Text informationVerificationFeedbackText;
        [SerializeField] private TMP_Text riskRecognitionFeedbackText;
        [SerializeField] private TMP_Text communityWarningFeedbackText;
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private TMP_Text totalFeedbackText;
        
        [Header("Labels (optional)")]
        [SerializeField] private string helpSeekingLabel = "Help Seeking";
        [SerializeField] private string pressureResistanceLabel = "Pressure Resistance";
        [SerializeField] private string informationVerificationLabel = "Information Verification";
        [SerializeField] private string riskRecognitionLabel = "Risk Recognition";
        [SerializeField] private string communityWarningLabel = "Community Warning";
        [SerializeField] private string totalLabel = "Total Score";

        [Header("Feedback Mapping")]
        [SerializeField] private ScoreFeedbackMappingSO scoreFeedbackMapping;

        [Header("Behavior")]
        [SerializeField] private bool startInactive = true;

        private void Awake()
        {
            if (startInactive)
                gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowFromContext()
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null) return;
            Show(ctx.PlayerState);
        }

        public void Show(PlayerStateService state)
        {
            if (state == null) return;
            gameObject.SetActive(true);
            UpdateScores(state);
        }

        private void UpdateScores(PlayerStateService state)
        {
            var helpSeeking = state.Get(GameStateKeys.ScoreHelpSeeking);
            var pressureResistance = state.Get(GameStateKeys.ScorePressureResistance);
            var informationVerification = state.Get(GameStateKeys.ScoreInformationVerification);
            var riskRecognition = state.Get(GameStateKeys.ScoreRiskRecognition);
            var communityWarning = state.Get(GameStateKeys.ScoreCommunityWarning);

            SetScoreWithFeedback(helpSeekingText, helpSeekingFeedbackText, helpSeekingLabel, GameStateKeys.ScoreHelpSeeking, helpSeeking);
            SetScoreWithFeedback(pressureResistanceText, pressureResistanceFeedbackText, pressureResistanceLabel, GameStateKeys.ScorePressureResistance, pressureResistance);
            SetScoreWithFeedback(informationVerificationText, informationVerificationFeedbackText, informationVerificationLabel, GameStateKeys.ScoreInformationVerification, informationVerification);
            SetScoreWithFeedback(riskRecognitionText, riskRecognitionFeedbackText, riskRecognitionLabel, GameStateKeys.ScoreRiskRecognition, riskRecognition);
            SetScoreWithFeedback(communityWarningText, communityWarningFeedbackText, communityWarningLabel, GameStateKeys.ScoreCommunityWarning, communityWarning);

            var total = Mathf.RoundToInt((helpSeeking + pressureResistance + informationVerification + riskRecognition + communityWarning) / 5f);
            SetScoreWithFeedback(totalScoreText, totalFeedbackText, totalLabel, ScoreFeedbackMappingSO.TotalScoreKey, total);
        }

        private void SetScoreWithFeedback(TMP_Text scoreText, TMP_Text feedbackText, string label, string scoreKey, int value)
        {
            if (scoreText != null)
            {
                var prefix = string.IsNullOrWhiteSpace(label) ? string.Empty : (label + ": ");
                scoreText.text = prefix + value.ToString();
            }

            var feedback = BuildFeedbackText(scoreKey, value);
            if (feedbackText != null)
            {
                feedbackText.text = feedback;
            }
            else if (scoreText != null && !string.IsNullOrWhiteSpace(feedback))
            {
                scoreText.text += "\n" + feedback;
            }
        }

        private string BuildFeedbackText(string scoreKey, int value)
        {
            var mapping = ResolveMapping();
            if (mapping == null) return string.Empty;

            if (!mapping.TryGetFeedback(scoreKey, value, out var entry) || entry == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(entry.ratingLabel))
                return entry.feedback ?? string.Empty;

            if (string.IsNullOrWhiteSpace(entry.feedback))
                return entry.ratingLabel;

            return entry.ratingLabel + ": " + entry.feedback;
        }

        private ScoreFeedbackMappingSO ResolveMapping()
        {
            if (scoreFeedbackMapping != null) return scoreFeedbackMapping;

            var ctx = GameBootstrap.Context;
            if (ctx != null && ctx.Config != null)
                return ctx.Config.scoreFeedbackMapping;

            return null;
        }

    }
}

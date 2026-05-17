using TMPro;
using UnityEngine;
using SeriousGame.App;
using SeriousGame.State;

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
        
        [Header("Labels (optional)")]
        [SerializeField] private string helpSeekingLabel = "Help Seeking";
        [SerializeField] private string pressureResistanceLabel = "Pressure Resistance";
        [SerializeField] private string informationVerificationLabel = "Information Verification";
        [SerializeField] private string riskRecognitionLabel = "Risk Recognition";
        [SerializeField] private string communityWarningLabel = "Community Warning";

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
            SetText(helpSeekingText, helpSeekingLabel, state.Get(GameStateKeys.ScoreHelpSeeking));
            SetText(pressureResistanceText, pressureResistanceLabel, state.Get(GameStateKeys.ScorePressureResistance));
            SetText(informationVerificationText, informationVerificationLabel, state.Get(GameStateKeys.ScoreInformationVerification));
            SetText(riskRecognitionText, riskRecognitionLabel, state.Get(GameStateKeys.ScoreRiskRecognition));
            SetText(communityWarningText, communityWarningLabel, state.Get(GameStateKeys.ScoreCommunityWarning));
        }

        private void SetText(TMP_Text text, string label, int value)
        {
            if (text == null) return;
            var prefix = string.IsNullOrWhiteSpace(label) ? string.Empty : (label + ": ");
            text.text = prefix + value.ToString();
        }

    }
}

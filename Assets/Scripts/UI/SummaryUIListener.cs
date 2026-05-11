using TMPro;
using UnityEngine;
using SeriousGame.App;
using SeriousGame.State;

namespace SeriousGame.UI
{
    public class SummaryUIListener : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text helpSeekingText;
        [SerializeField] private TMP_Text pressureResistanceText;
        [SerializeField] private TMP_Text informationVerificationText;
        [SerializeField] private TMP_Text riskRecognitionText;
        [SerializeField] private TMP_Text communityWarningText;

        private void OnEnable()
        {
            GameEventBus.OnSummaryRequested += HandleSummaryRequested;
        }

        private void OnDisable()
        {
            GameEventBus.OnSummaryRequested -= HandleSummaryRequested;
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void HandleSummaryRequested()
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null) return;

            UpdateScores(ctx.PlayerState);
            SetVisible(true);
        }

        private void UpdateScores(PlayerStateService state)
        {
            SetText(helpSeekingText, state.Get(GameStateKeys.ScoreHelpSeeking));
            SetText(pressureResistanceText, state.Get(GameStateKeys.ScorePressureResistance));
            SetText(informationVerificationText, state.Get(GameStateKeys.ScoreInformationVerification));
            SetText(riskRecognitionText, state.Get(GameStateKeys.ScoreRiskRecognition));
            SetText(communityWarningText, state.Get(GameStateKeys.ScoreCommunityWarning));
        }

        private void SetText(TMP_Text text, int value)
        {
            if (text != null)
                text.text = value.ToString();
        }

        private void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
                return;
            }

            gameObject.SetActive(visible);
        }
    }
}

using UnityEngine;
using SeriousGame.App;
using SeriousGame.Content;
using SeriousGame.UI;

namespace SeriousGame.Runtime
{
    public class EpisodeController : MonoBehaviour
    {
        [Header("Content")]
        public EpisodeSO episode;

        [Header("Runtime")]
        public BeatRunner beatRunner;

        [Header("UI")]
        public SummaryUI summaryUI;


        public OutcomeToastUI outcomeToast; // kéo vào Inspector

        private int _beatIndex;



        private void Start()
        {
            // Resolve episode from config if not assigned
            if (episode == null && GameBootstrap.Context != null && GameBootstrap.Context.Config != null)
                episode = GameBootstrap.Context.Config.defaultEpisode;

            if (episode == null)
            {
                Debug.LogError("[EpisodeController] No episode assigned.");
                return;
            }

            if (GameBootstrap.Context != null)
                GameBootstrap.Context.Session.Begin();

            _beatIndex = 0;
            RunCurrentBeat();
        }

        private void RunCurrentBeat()
        {
            var beats = episode.GetAllBeats();
            if (beats == null || beats.Length == 0)
            {
                Debug.LogError("[EpisodeController] Episode has no beats.");
                return;
            }

            if (_beatIndex < 0 || _beatIndex >= beats.Length)
            {
                EndChapter();
                return;
            }

            var beat = beats[_beatIndex];
            beatRunner.RunBeat(beat, OnChoiceResolved);
        }

        private void OnChoiceResolved(BeatSO beat, ChoiceSO choice)
        {
            var ctx = GameBootstrap.Context;
            var interaction = beat != null ? beat.GetPrimaryInteraction() : null;

            // Immediate outcome (optional)
            if (choice != null && !string.IsNullOrWhiteSpace(choice.outcomeText))
                Debug.Log($"[Outcome] {choice.outcomeText}");

            // Trace logging
            if (ctx != null && ctx.Trace != null)
                ctx.Trace.RecordChoice(ctx.Session.CurrentSessionId, episode, beat, interaction, choice);

            // Next beat rule
            if (beat != null && beat.endChapter)
            {
                EndChapter();
                return;
            }

            // If choice specifies nextBeatId, jump
            if (choice != null && !string.IsNullOrWhiteSpace(choice.nextBeatId))
            {
                var beats = episode.GetAllBeats();
                for (int i = 0; i < beats.Length; i++)
                {
                    if (beats[i] != null && beats[i].beatId == choice.nextBeatId)
                    {
                        _beatIndex = i;
                        RunCurrentBeat();
                        return;
                    }
                }
                Debug.LogWarning($"[EpisodeController] nextBeatId not found: {choice.nextBeatId}");
            }

            _beatIndex++;
            RunCurrentBeat();
        }

        private void EndChapter()
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.Feedback == null)
            {
                Debug.Log("[EpisodeController] EndChapter reached (no feedback service).");
                return;
            }

            var report = ctx.Feedback.GenerateEndChapterReport(ctx.Session.CurrentSessionId, episode);

            if (summaryUI != null)
                summaryUI.Show(report.rawSummaryText);
            else
                Debug.Log(report.rawSummaryText);
        }
    }
}

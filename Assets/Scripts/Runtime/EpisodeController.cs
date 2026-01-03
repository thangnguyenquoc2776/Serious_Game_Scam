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
        public static EpisodeController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }


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

        private bool _isRunningWorldBeat = false; // Thêm biến để đánh dấu

        public void RunBeatFromWorld(BeatSO beat)
        {
            if (beat == null) return;

            _isRunningWorldBeat = true; // Đang chạy beat "ngoài luồng"
            beatRunner.RunBeat(beat, OnChoiceResolved);
        }

        public void OnChoiceResolved(BeatSO beat, ChoiceSO choice)
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
            // Nếu đây là beat kết thúc chương
    if (beat != null && beat.endChapter)
    {
        EndChapter();
        return;
    }

    // Xử lý logic nhảy Beat nếu có nextBeatId
    if (choice != null && !string.IsNullOrWhiteSpace(choice.nextBeatId))
    {
        JumpToBeat(choice.nextBeatId);
        return;
    }

    // QUAN TRỌNG: Nếu vừa tương tác từ World xong, không tự động chạy beat tiếp theo trong list
    if (_isRunningWorldBeat)
    {
        _isRunningWorldBeat = false; 
        Debug.Log("[EpisodeController] World interaction finished. Standing by.");
        return; 
    }

    // Nếu đang chạy truyện bình thường thì mới tăng Index
    _beatIndex++;
    RunCurrentBeat();
        }

private void JumpToBeat(string id)
{
    var beats = episode.GetAllBeats();
    for (int i = 0; i < beats.Length; i++)
    {
        if (beats[i] != null && beats[i].beatId == id)
        {
            _beatIndex = i;
            _isRunningWorldBeat = false; // Quay lại luồng chính
            RunCurrentBeat();
            return;
        }
    }
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

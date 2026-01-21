using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Runtime
{
    public class EpisodeController : MonoBehaviour
    {
        public static EpisodeController Instance;

        [Header("Content")]
        public EpisodeSO episode;

        [Header("Runtime")]
        public BeatRunner beatRunner;

        private BeatSO[] _beats;
        private int _beatIndex = 0;

        private bool waitingForInteract = false;
        private bool isInteractionRunning = false;
        public bool chapterEnded = false;


        public Transform SitAnchor;
        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _beats = episode.GetAllBeats();
            _beatIndex = 0;

            Debug.Log("[Episode] Start");
            TryRunCurrentBeat();
        }

        public BeatSO GetCurrentBeat() // lấy beat hiện tại
        {
            if (_beatIndex < 0 || _beatIndex >= _beats.Length)
                return null;
            return _beats[_beatIndex];
        }

        void TryRunCurrentBeat() // chạy beat hiện tại
        {
            var beat = GetCurrentBeat();
            if (beat == null)
            {
                Debug.Log("[Episode] No more beats. Episode finished.");
                chapterEnded = true;
                // Gọi UI tổng kết nếu có
                if (EpisodeSummaryUI.Instance != null)
                {
                    EpisodeSummaryUI.Instance.ShowSummary();
                }
                return;
            }

            Debug.Log($"[Episode] TryRun Beat {beat.beatId}");

            waitingForInteract = false;
            isInteractionRunning = false;

            if (beat.autoStart) // nếu autoStart = true thì chạy luôn
            {
                Debug.Log("[Episode] autoStart");
                RunMainInteraction(beat);
            }
            else if (beat.requireInteract) // nếu requireInteract = true thì chờ tương tác
            {
                waitingForInteract = true;
                Debug.Log("[Episode] waitingForInteract = TRUE");
            }
        }


        public void OnWorldInteract(string interactId)
        {
            if (!waitingForInteract)
            {
                Debug.Log("[Episode] Ignore interact – not waiting");
                return;
            }

            var beat = GetCurrentBeat();
            if (beat == null) return;

            if (beat.beatId != interactId)
            {
                Debug.Log("[Episode] Wrong interact target");
                return;
            }

            waitingForInteract = false;
            RunMainInteraction(beat);
        }

        void RunMainInteraction(BeatSO beat)
        {
            if (isInteractionRunning)
            {
                Debug.Log("[Episode] Interaction already running");
                return;
            }

            isInteractionRunning = true;
            Debug.Log("[Episode] Run main interaction");

            if (beat.beatId == "B01" || beat.beatId == "B07")
            {
                var player = FindAnyObjectByType<PlayerController>();

                if (player != null && SitAnchor != null)
                {
                    player.SitAt(SitAnchor);
                }
            }
            else if (beat.beatId == "B02")
            {
                var lead = FindAnyObjectByType<LeadController>();
                if (lead != null)
                {
                    lead.GoTalkThenReturn(beat.interaction, () =>
                    {
                        isInteractionRunning = false;
                        AdvanceBeat();
                    });
                    return;
                }   
            }
            beatRunner.RunBeat(beat, OnChoiceResolved);
        }

            void OnChoiceResolved(BeatSO beat, ChoiceSO choice)
            {
                if (choice != null && choice.outcomeInteraction != null)
                {
                    Debug.Log("[Episode] Run outcome interaction");

                    beatRunner.RunInteraction(choice.outcomeInteraction, () =>
                    {
                        ResolveChoice(choice);
                    });
                }
                else
                {
                    ResolveChoice(choice);
                }
            }

            void ResolveChoice(ChoiceSO choice)
            {
                isInteractionRunning = false;

                // Nếu Choice có stateToTrigger, cập nhật GameState
                if (choice != null && !string.IsNullOrEmpty(choice.stateToTrigger))
                {
                    if (GameStateManager.Instance != null)
                    {
                        GameStateManager.Instance.SetFlag(choice.stateToTrigger, true);
                    }
                    else
                    {
                        Debug.LogWarning("[Episode] GameStateManager.Instance is null while trying to set state from choice.");
                    }
                }

                if (choice != null && choice.nextBeat != null)
                {
                    JumpToBeat(choice.nextBeat);
                }
                else
                {
                    AdvanceBeat();
                }
            }

            void AdvanceBeat()
            {
                _beatIndex++;
                TryRunCurrentBeat();
            }

            void JumpToBeat(BeatSO beat)
            {
                for (int i = 0; i < _beats.Length; i++)
                {
                    if (_beats[i] == beat)
                    {
                        _beatIndex = i;
                        TryRunCurrentBeat();
                        return;
                    }
                }

                Debug.LogError("[Episode] Jump beat not found");
            }
        }
    }

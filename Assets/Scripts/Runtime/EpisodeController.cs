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

        BeatSO GetCurrentBeat()
        {
            if (_beatIndex < 0 || _beatIndex >= _beats.Length)
                return null;
            return _beats[_beatIndex];
        }

        void TryRunCurrentBeat()
        {
            var beat = GetCurrentBeat();
            Debug.Log($"[Episode] TryRun Beat {beat.beatId}");

            waitingForInteract = false;
            isInteractionRunning = false;

            if (beat.autoStart)
            {
                Debug.Log("[Episode] autoStart");
                RunMainInteraction(beat);
            }
            else if (beat.requireInteract)
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

            if (beat.interactTargetId != interactId)
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

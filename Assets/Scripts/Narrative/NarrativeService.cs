using UnityEngine;
using Yarn.Unity;
using SeriousGame.App;
using SeriousGame.Runtime;

namespace SeriousGame.Narrative
{
    public class NarrativeService
    {
        private readonly AppContext _context;
        private DialogueRunner _runner;
        private PlayerController _player;

        public NarrativeService(AppContext context)
        {
            _context = context;
        }

        private void OnDialogueStarted()
        {   
            if (_player != null)
            {
                // Khoá di chuyển, xoay chuột, hiện Cursor
                _player.SetLockState(true);
                Debug.Log("[NarrativeService] Dialogue started, locking player controls.");
            }
        }

        private void OnDialogueEnded()
        {
            if (_player != null)
            {
                // Mở lại di chuyển, ẩn Cursor để tiếp tục chơi
                _player.SetLockState(false);
                Debug.Log("[NarrativeService] Dialogue ended, unlocking player controls.");
            }
        }

        public string CurrentNode { get; private set; }

        public void BindRunner(DialogueRunner runner)
        {
            if (_runner == runner) return;
            UnbindRunner();

            _runner = runner;
            if (_runner != null)
            {
                _runner.onDialogueStart.AddListener(OnDialogueStarted);
                _runner.onDialogueComplete.AddListener(OnDialogueEnded);
            }
        }

        public void UnbindRunner()
        {
            if (_runner == null) return;
            _runner.onDialogueStart.RemoveListener(OnDialogueStarted);
            _runner.onDialogueComplete.RemoveListener(OnDialogueEnded);
            _runner = null;
        }

        public void BindPlayer(PlayerController player)
        {
            _player = player;
        }

        public void SetCurrentNode(string nodeName)
        {
            CurrentNode = nodeName;
        }

        public void StartNode(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName)) return;
            if (_runner == null)
            {
                Debug.LogWarning("[NarrativeService] DialogueRunner not bound.");
                return;
            }

            CurrentNode = nodeName;
            _runner.StartDialogue(nodeName);
        }

        public AppContext GetContext()
        {
            return _context;
        }
        public void Dispose()
        {
            UnbindRunner();
        }
    }
}

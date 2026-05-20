using UnityEngine;
using SeriousGame.Runtime;

namespace SeriousGame.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        private const string PauseLockSource = "PauseMenu";

        [Header("UI")]
        [SerializeField] private GameObject menuRoot;

        [Header("UI Blocking")]
        [SerializeField] private CanvasGroup[] blockOnPause;

        [Header("Flow")]
        [SerializeField] private UIFlowManager flowManager;

        [Header("Optional Player")]
        [SerializeField] private PlayerController player;
        [SerializeField] private bool pauseTime = true;

        private bool uiBlocked;
        private bool[] cachedInteractable;
        private bool[] cachedBlocksRaycasts;

        private void Awake()
        {
            if (menuRoot != null)
                menuRoot.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        private void OnDisable()
        {
            if (pauseTime)
                Time.timeScale = 1f;
            SetUiBlocked(false);
        }

        public void TogglePause()
        {   
            if (menuRoot == null) return;
            if (menuRoot.activeSelf)
                Resume();
            else
                Pause();
        }

        public void Pause()
        {
            if (menuRoot != null)
                menuRoot.SetActive(true);
            if (pauseTime)
                Time.timeScale = 0f;
            if (player != null)
                player.SetLockState(PauseLockSource, true);
            SetUiBlocked(true);
        }

        public void Resume()
        {
            if (menuRoot != null)
                menuRoot.SetActive(false);
            if (pauseTime)
                Time.timeScale = 1f;
            if (player != null)
                player.SetLockState(PauseLockSource, false);
            SetUiBlocked(false);
        }

        private void SetUiBlocked(bool blocked)
        {
            if (blockOnPause == null || blockOnPause.Length == 0)
                return;

            if (blocked)
            {
                if (uiBlocked) return;
                uiBlocked = true;
                cachedInteractable = new bool[blockOnPause.Length];
                cachedBlocksRaycasts = new bool[blockOnPause.Length];

                for (int i = 0; i < blockOnPause.Length; i++)
                {
                    var cg = blockOnPause[i];
                    if (cg == null) continue;
                    cachedInteractable[i] = cg.interactable;
                    cachedBlocksRaycasts[i] = cg.blocksRaycasts;
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }
                return;
            }

            if (!uiBlocked) return;
            for (int i = 0; i < blockOnPause.Length; i++)
            {
                var cg = blockOnPause[i];
                if (cg == null) continue;
                cg.interactable = cachedInteractable != null && i < cachedInteractable.Length && cachedInteractable[i];
                cg.blocksRaycasts = cachedBlocksRaycasts != null && i < cachedBlocksRaycasts.Length && cachedBlocksRaycasts[i];
            }

            uiBlocked = false;
        }

        public void QuitToMenu()
        {
            if (pauseTime)
                Time.timeScale = 1f;

            if (flowManager != null)
                flowManager.QuitToMenu();
        }

        public void QuitGame()
        {
            if (pauseTime)
                Time.timeScale = 1f;

            if (flowManager != null)
                flowManager.QuitGame();
        }
    }
}

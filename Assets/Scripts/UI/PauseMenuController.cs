using UnityEngine;
using SeriousGame.Runtime;

namespace SeriousGame.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject menuRoot;

        [Header("Flow")]
        [SerializeField] private UIFlowManager flowManager;

        [Header("Optional Player")]
        [SerializeField] private PlayerController player;
        [SerializeField] private bool pauseTime = true;

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
                player.SetLockState(true);
        }

        public void Resume()
        {
            if (menuRoot != null)
                menuRoot.SetActive(false);
            if (pauseTime)
                Time.timeScale = 1f;
            if (player != null)
                player.SetLockState(false);
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

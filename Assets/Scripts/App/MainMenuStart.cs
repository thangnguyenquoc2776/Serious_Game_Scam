// // Assets/Scripts/App/MainMenuStart.cs
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using SeriousGame.UI;

// namespace SeriousGame.App
// {
//     public class MainMenuStart : MonoBehaviour
//     {
//         [SerializeField] private UIFlowManager flowManager;

//         public void StartDemo()
//         {
//             if (flowManager != null)
//             {
//                 flowManager.StartGame();
//                 return;
//             }

//             var ctx = GameBootstrap.Context;
//             var cfg = ctx != null ? ctx.Config : null;
//             var target = cfg != null ? cfg.demoEpisodeSceneName : null;

//             if (string.IsNullOrWhiteSpace(target))
//             {
//                 Debug.LogError("[MainMenuStart] Không xác định được scene để start demo (Config thiếu?).");
//                 return;
//             }

//             SceneManager.LoadScene(target);
//         }
//     }
// }

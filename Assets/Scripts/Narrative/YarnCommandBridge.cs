using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using SeriousGame.App;
using SeriousGame.Trace;
using SeriousGame.State;
using SeriousGame.Runtime;

namespace SeriousGame.Narrative
{
    public class YarnCommandBridge : MonoBehaviour
    {
        [Header("Context (optional override)")]
        // [SerializeField] private AppContext contextOverride;

        [Header("Yarn")]
        [SerializeField] private DialogueRunner runner;

        [Header("Optional Player")]
        [SerializeField] private PlayerController player;

        // private AppContext Context => contextOverride != null ? contextOverride : GameBootstrap.Context;
        private static AppContext Context => GameBootstrap.Context;

        private void Awake()
        {
            var ctx = Context;
            if (ctx != null && ctx.Narrative != null)
            {
                if (runner != null)
                    ctx.Narrative.BindRunner(runner);
                if (player != null)
                    ctx.Narrative.BindPlayer(player);
            }
        }

        private void OnDestroy()
        {
            var ctx = Context;
            if (ctx != null && ctx.Narrative != null)
                ctx.Narrative.UnbindRunner();
        }

        [YarnCommand("trace")]
        public static void TraceCommand(string traceTypeId, int choiceid)
        {
            var ctx = Context;
            if (ctx == null || ctx.Trace == null) return;

            var sessionId = ctx.Session != null ? ctx.Session.CurrentSessionId : "";
            var yarnNode = ctx.Narrative != null ? ctx.Narrative.CurrentNode : "";
            var milestoneId = ctx.Narrative != null ? ctx.Narrative.CurrentMilestoneId : "";
            var routeId = string.IsNullOrWhiteSpace(yarnNode) ? "Node_Unknown" : yarnNode;
            var trace = ctx.Trace.BuildTrace(
                sessionId,
                milestoneId,
                routeId,
                choiceid,
                traceTypeId, // time stamp bên ham buildtrace co them
                ctx.PlayerState != null ? ctx.PlayerState.GetSnapshot() : null);

            ctx.Trace.LogEvent(trace);
            if (ctx.Feedback != null)
                ctx.Feedback.ApplyTrace(traceTypeId);
        }

        [YarnCommand("set_node")]
        public static void SetNode(string nodeName)
        {
            var ctx = Context;
            if (ctx == null || ctx.Narrative == null) return;
            ctx.Narrative.SetCurrentNode(nodeName);
        }

        [YarnCommand("set_milestone")]
        public static void SetMilestone(int milestoneId)
        {
            var ctx = Context;
            if (ctx == null || ctx.Narrative == null) return;
            ctx.Narrative.SetCurrentMilestone(milestoneId.ToString());
        }

        [YarnCommand("set_objective")]
        public static void SetObjective(string objectiveId)
        {
            var ctx = Context;
            if (ctx == null)
            {
                Debug.LogWarning("[YarnCommandBridge] GameBootstrap.Context is null; cannot set objective.");
                return;
            }
            if (ctx.Quest == null)
            {
                Debug.LogWarning("[YarnCommandBridge] QuestService is not available (missing on GameBootstrap object); cannot set objective.");
                return;
            }

            ctx.Quest.SetObjective(objectiveId);
        }

         [YarnCommand("chapter_end")]
        public static void ChapterEnd(string chapterId = "")
        {
            var ctx = Context;
            if (ctx == null || ctx.Trace == null || ctx.Session == null) return;

            _ = ctx.Trace.SendSessionData(true, chapterId);
            SeriousGame.App.GameEventBus.RaiseSummaryRequested();
        }

        [YarnCommand("add_state")]
        public static void AddState(string key, int delta)
        {
            var ctx = Context;
            if (ctx == null || ctx.PlayerState == null) return;
            ctx.PlayerState.Add(key, delta);
        }

        [YarnCommand("load_scene")]
        public static void LoadScene(string sceneName)
        {
            var ctx = Context;
            if (ctx == null || ctx.Scenes == null) return;
            ctx.Scenes.Load(sceneName);
        }

        [YarnCommand("show_summary")]
        public static void ShowSummary()
        {
            SeriousGame.App.GameEventBus.RaiseSummaryRequested();
        }

        [YarnCommand("save_game")]
        public static void SaveGame()
        {
            var ctx = Context;
            if (ctx == null || ctx.Save == null) return;

            var episodeId = SceneManager.GetActiveScene().name;

            var currentNode = ctx.Narrative != null ? ctx.Narrative.CurrentNode : "";
            var currentMilestone = ctx.Narrative != null ? ctx.Narrative.CurrentMilestoneId : "";
            ctx.Save.SaveCurrent(episodeId, currentNode, currentMilestone);
        }

        // [YarnCommand("show_phone_chat")]
        // public static void ShowPhoneChat(string interactionId)
        // {
        //     if (string.IsNullOrWhiteSpace(interactionId)) return;
        //     SeriousGame.App.GameEventBus.RaisePhoneChatRequested(interactionId);
        // }

        // [YarnCommand("show_pc_chat")]
        // public static void ShowPcChat(string interactionId)
        // {
        //     if (string.IsNullOrWhiteSpace(interactionId)) return;
        //     SeriousGame.App.GameEventBus.RaisePcChatRequested(interactionId);
        // }

        [YarnCommand("switch_ui")]
        public static void SwitchUI(string uitype)
        {
            SeriousGame.App.GameEventBus.RaiseSwitchUIRequested(uitype);
        }

        // [YarnCommand("show_chat")]
        // public static void ShowChat(string characterName, string message)
        // {
        //     if (string.IsNullOrWhiteSpace(message)) return;
        //     SeriousGame.App.GameEventBus.RaisePhoneMessageReceived(characterName, message);
        // }

        [YarnCommand("show_hint")]
        public static void ShowHint(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            SeriousGame.App.GameEventBus.RaiseHintRequested(message);
        }

        [YarnCommand("set_state")]
        public static void SetState(string key, int value)
        {
            var ctx = Context;
            if (ctx == null || ctx.PlayerState == null) return;
            ctx.PlayerState.Set(key, value);
        }

        [YarnCommand("set_flag")]
        public static void SetFlag(string key, bool value = true)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var ctx = Context;
            if (ctx == null || ctx.PlayerState == null) return;
            ctx.PlayerState.SetFlag(key, value);
        }
     

        [YarnCommand("clear_flag")]
        public static void ClearFlag(string key)
        {
            SetFlag(key, false);
        }

        [YarnFunction("get_state")]
        public static int GetState(string key)
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null) return 0;
            return ctx.PlayerState.Get(key);
        }

        [YarnFunction("get_flag")]
        public static bool GetFlag(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null) return false;
            return ctx.PlayerState.CheckFlag(key);
        }

        [YarnCommand("show_toast")]
        public static void ShowToast(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            SeriousGame.App.GameEventBus.RaiseToastRequested(message);
        }

        // [YarnCommand("test_func")]
        // public static void test_func(string message)
        // {
        //     if (string.IsNullOrWhiteSpace(message)) return;
        //     Debug.Log($"[YarnCommandBridge] test_func called with message: {message}");
        // }
    }
}

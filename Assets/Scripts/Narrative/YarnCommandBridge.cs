using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using SeriousGame.App;
using SeriousGame.Trace;
using SeriousGame.State;
using SeriousGame.Runtime;
using AppContext = SeriousGame.App.AppContext;

namespace SeriousGame.Narrative
{
    public class YarnCommandBridge : MonoBehaviour
    {
        [Serializable]
        private class TeleportPoint
        {
            public string id;
            public Transform target;
        }

        [Serializable]
        private class SeatPoint
        {
            public string id;
            public Transform seat;
            public Vector3 positionOffset;
            public Vector3 rotationOffsetEuler;
            public Vector3 cameraLocalOffset;
        }
        [Header("Context (optional override)")]
        // [SerializeField] private AppContext contextOverride;

        [Header("Yarn")]
        [SerializeField] private DialogueRunner runner;

        [Header("Optional Player")]
        [SerializeField] private PlayerController player;

        [Header("Teleport Points")]
        [SerializeField] private TeleportPoint[] teleportPoints;

        [Header("Seat Points")]
        [SerializeField] private SeatPoint[] seatPoints;

        // private AppContext Context => contextOverride != null ? contextOverride : GameBootstrap.Context;
        private static AppContext Context => GameBootstrap.Context;
        private static YarnCommandBridge _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            var ctx = Context;
            if (ctx != null && ctx.Narrative != null)
            {
                if (runner != null)
                    ctx.Narrative.BindRunner(runner);
                if (player != null)
                    ctx.Narrative.BindPlayer(player);
            }
            player.SetLockState(true); // Lock player controls until narrative starts
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            var ctx = Context;
            if (ctx != null && ctx.Narrative != null)
                ctx.Narrative.UnbindRunner();
        }

        [YarnCommand("fade_in_out")]
        public static void FadeInOut(float fadeSeconds = 1f, float holdSeconds = 0f)
        {
            if (ScreenFader.Instance == null) return;
            ScreenFader.Instance.FadeOutInHold(fadeSeconds, holdSeconds);
        }

        [YarnCommand("fade_in")]
        public static void FadeIn(float fadeSeconds = 1f)
        {
            if (ScreenFader.Instance == null) return;
            ScreenFader.Instance.FadeOut(fadeSeconds);
        }

        [YarnCommand("fade_out")]
        public static void FadeOut(float fadeSeconds = 1f)
        {
            if (ScreenFader.Instance == null) return;
            ScreenFader.Instance.FadeIn(fadeSeconds);
        }

        [YarnCommand("teleport_to")]
        public static void TeleportTo(string pointId)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[YarnCommandBridge] TeleportTo failed: bridge instance missing.");
                return;
            }

            var target = _instance.ResolveTeleportPoint(pointId);
            if (target == null)
            {
                Debug.LogWarning($"[YarnCommandBridge] TeleportTo failed: point '{pointId}' not found.");
                return;
            }

            var pc = _instance.ResolvePlayer();
            if (pc == null)
            {
                Debug.LogWarning("[YarnCommandBridge] TeleportTo failed: PlayerController not found.");
                return;
            }

            pc.TeleportTo(target);
        }

        [YarnCommand("sit")]
        public static void Sit(string seatId)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[YarnCommandBridge] Sit failed: bridge instance missing.");
                return;
            }

            var seat = _instance.ResolveSeatPoint(seatId);
            if (seat == null || seat.seat == null)
            {
                Debug.LogWarning($"[YarnCommandBridge] Sit failed: seat '{seatId}' not found.");
                return;
            }

            var pc = _instance.ResolvePlayer();
            if (pc == null)
            {
                Debug.LogWarning("[YarnCommandBridge] Sit failed: PlayerController not found.");
                return;
            }

            pc.Sit(seat.seat, seat.positionOffset, seat.rotationOffsetEuler, seat.cameraLocalOffset);
        }

        [YarnCommand("unsit")]
        public static void UnSit()
        {
            if (_instance == null) return;
            var pc = _instance.ResolvePlayer();
            if (pc == null) return;
            pc.UnSit();
        }

        [YarnCommand("trace")]
        public static void TraceCommand(string traceTypeId, int choiceid, string objectName = "")
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
                objectName,
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

        private Transform ResolveTeleportPoint(string pointId)
        {
            if (teleportPoints == null || string.IsNullOrWhiteSpace(pointId)) return null;
            foreach (var entry in teleportPoints)
            {
                if (entry == null || entry.target == null) continue;
                if (!string.Equals(entry.id, pointId, StringComparison.OrdinalIgnoreCase)) continue;
                return entry.target;
            }
            return null;
        }

        private SeatPoint ResolveSeatPoint(string seatId)
        {
            if (seatPoints == null || string.IsNullOrWhiteSpace(seatId)) return null;
            foreach (var entry in seatPoints)
            {
                if (entry == null || entry.seat == null) continue;
                if (!string.Equals(entry.id, seatId, StringComparison.OrdinalIgnoreCase)) continue;
                return entry;
            }
            return null;
        }

        private PlayerController ResolvePlayer()
        {
            if (player != null) return player;

#if UNITY_2023_1_OR_NEWER || UNITY_2022_2_OR_NEWER
            return FindAnyObjectByType<PlayerController>();
#else
            return FindObjectOfType<PlayerController>();
#endif
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

        // [YarnCommand("show_hint")]
        // public static void ShowHint(string message)
        // {
        //     if (string.IsNullOrWhiteSpace(message)) return;
        //     SeriousGame.App.GameEventBus.RaiseHintRequested(message);
        // }

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

        // [YarnCommand("show_toast")]
        // public static void ShowToast(string message)
        // {
        //     if (string.IsNullOrWhiteSpace(message)) return;
        //     SeriousGame.App.GameEventBus.RaiseToastRequested(message);
        // }

    }
}

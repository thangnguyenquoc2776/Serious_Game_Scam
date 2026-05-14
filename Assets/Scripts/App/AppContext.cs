using UnityEngine;
using SeriousGame.Content;
using SeriousGame.Trace;
using SeriousGame.Feedback;
using SeriousGame.State;
using SeriousGame.Save;
using SeriousGame.Narrative;
using SeriousGame.Auth;

namespace SeriousGame.App
{
    public class AppContext : MonoBehaviour
    {
        public AppConfigSO Config { get; private set; }
        public SceneService Scenes { get; private set; }
        public SessionService Session { get; private set; }
        public PlayerStateService PlayerState { get; private set; }
        public TraceService Trace { get; private set; }
        public AuthService Auth { get; private set; }
        public FeedbackService Feedback { get; private set; }
        public SaveService Save { get; private set; }
        public NarrativeService Narrative { get; private set; }
        public QuestService Quest { get; private set; }
    
        public void Init(AppConfigSO config)
        {
            Config = config;

            Scenes = new SceneService();
            Session = new SessionService();
            PlayerState = new PlayerStateService();

            var store = new InMemoryTraceStore();
            var fileStore = new FileTraceStore();
            Trace = new TraceService(store, fileStore, Session, PlayerState, config);

            Auth = new AuthService(config != null ? config.firebaseWebApiKey : null);

            // Narrative context should exist before SaveService so save/load can include node and milestone.
            Narrative = GetComponent<NarrativeService>();
            if (Narrative == null)
            {
                Debug.LogWarning("[AppContext] NarrativeService missing on GameBootstrap; adding a default one at runtime.");
                Narrative = gameObject.AddComponent<NarrativeService>();
            }

            Quest = GetComponent<QuestService>();
            if (Quest == null)
            {
                Debug.LogWarning("[AppContext] QuestService missing on GameBootstrap; adding a default one at runtime. (Objective toast map will be unassigned.)");
                Quest = gameObject.AddComponent<QuestService>();
            }
            Feedback = new FeedbackService(
                Trace,
                config != null ? config.evidenceMapping : null,
                PlayerState);
            Save = new SaveService(Session, PlayerState, Narrative);
        }

        private void OnDestroy()
        {
            if (Narrative != null)
                Narrative.Dispose();
        }
    }
}

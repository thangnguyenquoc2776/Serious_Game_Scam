using UnityEngine;
using SeriousGame.Content;
using SeriousGame.Trace;
using SeriousGame.Feedback;

namespace SeriousGame.App
{
    public class AppContext : MonoBehaviour
    {
        public AppConfigSO Config { get; private set; }
        public SceneService Scenes { get; private set; }
        public SessionService Session { get; private set; }
        public TraceService Trace { get; private set; }
        public FeedbackService Feedback { get; private set; }

        public void Init(AppConfigSO config)
        {
            Config = config;

            Scenes = new SceneService();
            Session = new SessionService();

            var store = new InMemoryTraceStore();
            Trace = new TraceService(store);

            Feedback = new FeedbackService(Trace, config != null ? config.traceTaxonomy : null);
        }
    }
}

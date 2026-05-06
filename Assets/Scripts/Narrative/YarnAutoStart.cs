using UnityEngine;
using SeriousGame.App;

namespace SeriousGame.Narrative
{
    public class YarnAutoStart : MonoBehaviour
    {
        public string nodeName;
        public bool startOnEnable = true;

        private void Start()
        {
            if (startOnEnable)
                StartNode();
        }

        public void StartNode()
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.Narrative == null) return;
            ctx.Narrative.StartNode(nodeName);
        }
    }
}

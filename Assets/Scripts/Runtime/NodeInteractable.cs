using UnityEngine;
using SeriousGame.App;

namespace SeriousGame.Runtime
{
    public class NodeInteractable : MonoBehaviour, IInteractable
    {
        [Header("Yarn Node")]
        public string nodeName;

        public void Interact()
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                Debug.LogWarning("[NodeInteractable] nodeName is empty.");
                return;
            }

            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.Narrative == null)
            {
                Debug.LogWarning("[NodeInteractable] NarrativeService not available.");
                return;
            }

            ctx.Narrative.StartNode(nodeName);
        }
    }
}

using UnityEngine;
using SeriousGame.App;
using System;
using System.Collections.Generic;

namespace SeriousGame.Runtime
{   
    [Serializable]
    public class ObjectiveNodeMap
    {
        public string objectiveId; // Mã nhiệm vụ (ví dụ: OBJ_TALK_GUARD_1)
        public string yarnNode;    // Node tương ứng (ví dụ: Guard_FirstTime)
    }
    public class NodeInteractable : MonoBehaviour, IInteractable
    {
        [Header("Yarn Node")]
        // public string nodeName;
        [Header("Danh sách ánh xạ Nhiệm vụ -> Hội thoại")]
        public List<ObjectiveNodeMap> nodeMappings;
        public string fallbackNode;  // Ví dụ: Friend_Busy_Dialogue

        // [Header("Điều kiện Nhiệm vụ")]
        // [Tooltip("Mã nhiệm vụ bắt buộc phải có để chạy Main Node")]
        // public string requiredObjectiveId; // Ví dụ: OBJ_FIND_FRIEND

        public void Interact()
        {
            // if (string.IsNullOrWhiteSpace(nodeName))
            // {
            //     Debug.LogWarning("[NodeInteractable] nodeName is empty.");
            //     return;
            // }

            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.Narrative == null)
            {
                Debug.LogWarning("[NodeInteractable] NarrativeService not available.");
                return;
            }
            var currentObjectiveId = ctx.Quest != null ? ctx.Quest.CurrentObjectiveId : null;
            
            var match = nodeMappings.Find(m => m.objectiveId == currentObjectiveId);

            if (match != null)
            {
                GameBootstrap.Context.Narrative.StartNode(match.yarnNode);
            }
            else
            {
                GameBootstrap.Context.Narrative.StartNode(fallbackNode);
                // Debug.Log("Chua duoc tuong tac bay gio");
            }
        }
    }
}

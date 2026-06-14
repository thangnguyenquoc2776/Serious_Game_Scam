using System.Collections.Generic;
using UnityEngine;
using SeriousGame.App;
using SeriousGame.Runtime;

// Interactable dùng để trigger Yarn node theo objective khi player đi vào vùng.
public class TeleportInteractable : MonoBehaviour
{
    [Header("Điều kiện (tuỳ chọn)")]
    [Tooltip("Nếu có, chỉ cho teleport khi flag này thoả điều kiện.")]
    public string requiredFlagKey;       // ví dụ: "PASSED_GUARD"
    public bool requiredValue = true;

    [Header("Objective -> Yarn Node (tuỳ chọn)")]
    public List<ObjectiveNodeMap> nodeMappings = new List<ObjectiveNodeMap>();
    public string fallbackNode;

    [Header("One-shot?")]
    public bool oneShot = true;
    private bool _used = false;

    void OnTriggerEnter(Collider other)
    {
        // Kích hoạt tự động khi player bước vào vùng trigger
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        TryTrigger();
    }

    void TryTrigger()
    {
        if (oneShot && _used) return;

        // Nếu có điều kiện state thì kiểm tra trước
        if (!string.IsNullOrEmpty(requiredFlagKey))
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null)
            {
                Debug.LogWarning("[TeleportInteractable] PlayerState is not available.");
                return;
            }

            bool current = ctx.PlayerState.CheckFlag(requiredFlagKey);
            if (current != requiredValue)
            {
                Debug.Log("[TeleportInteractable] Condition not met: " + requiredFlagKey);
                return; // chưa qua được bảo vệ thì chưa cho vào office
            }
        }

        TryStartNodeForObjective();

        _used = true;
    }

    private void TryStartNodeForObjective()
    {
        var ctx = GameBootstrap.Context;
        if (ctx == null || ctx.Narrative == null) return;

        var currentObjectiveId = ctx.Quest != null ? ctx.Quest.CurrentObjectiveId : null;
        ObjectiveNodeMap match = null;

        if (nodeMappings != null && !string.IsNullOrWhiteSpace(currentObjectiveId))
            match = nodeMappings.Find(m => m.objectiveId == currentObjectiveId);

        if (match != null && !string.IsNullOrWhiteSpace(match.yarnNode))
        {
            ctx.Narrative.StartNode(match.yarnNode);
            return;
        }

        if (!string.IsNullOrWhiteSpace(fallbackNode))
            ctx.Narrative.StartNode(fallbackNode);
    }
}

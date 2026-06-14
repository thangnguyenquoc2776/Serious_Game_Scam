using UnityEngine;

namespace SeriousGame.Runtime
{
    public class WallCleanupTrigger : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private bool requireRigidbody = false;

        private void OnTriggerEnter(Collider other)
        {
            if (requireRigidbody && other.attachedRigidbody == null) return;

            // Tìm component Teleport trên NPC vừa va chạm
            var target = other.GetComponentInParent<NPCWallTeleport>();
            if (target == null) return;

            // Gọi hàm đưa NPC quay về điểm xuất phát
            target.TeleportToStart();
        }
    }
}
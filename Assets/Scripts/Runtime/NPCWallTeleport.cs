using UnityEngine;

namespace SeriousGame.Runtime
{
    public class NPCWallTeleport : MonoBehaviour
    {
        [Header("Teleport Settings")]
        [Tooltip("Kéo Transform của điểm xuất phát (Điểm A) vào đây")]
        [SerializeField] private Transform startPoint; 
        
        [Tooltip("Độ lệch vị trí nếu bạn không muốn NPC sinh ra y chang chỗ cũ")]
        // [SerializeField] private Vector3 spawnOffset = Vector3.zero;

        private Rigidbody _rb;

        private void Awake()
        {
            // Lấy sẵn Rigidbody để tối ưu, không cần gọi GetComponent nhiều lần
            _rb = GetComponent<Rigidbody>();
        }

        public void TeleportToStart()
        {
            if (startPoint == null)
            {
                Debug.LogWarning($"[NPCWallTeleport] Chưa gán Start Point cho {gameObject.name}!");
                return;
            }

            // 1. Đưa vị trí về điểm xuất phát + độ lệch (nếu có)
            Vector3 newPosition = new Vector3(startPoint.position.x, transform.position.y, startPoint.position.z);
    
            // Gán lại cho transform.position
            transform.position = newPosition;

            // // 2. Reset vận tốc vật lý (Cực kỳ quan trọng nếu NPC dùng Rigidbody)
            // if (_rb != null)
            // {
            //     _rb.velocity = Vector3.zero;
            //     _rb.angularVelocity = Vector3.zero;
            // }
        }
    }
}